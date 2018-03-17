using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDDN;
using EnlEliteBot.Web.EDSM;
using EnlEliteBot.Web.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.MarketQueries
{
    /// <summary>
    /// Class to answer questions related to commodities...
    /// </summary>
    public static class MarketQuery
    {

        private const byte MIN_TRADES_PER_STATION = 4;

        public static async Task<MarketResult> GetPurchaseStation(BGSProfitRequest request)
        {

            var timer = Stopwatch.StartNew();
            //lets start the EDSM ajax call, it might take a few seconds
            var possibleSourceTask = EDSMHelper.GetSystemsInSphereAround(request.SaleSystem, request.RadiusLY);


            var key = $"Market-{request.SaleSystem}-{request.SaleMarket}";
            var destStationRaw = await RedisHelper.Database.StringGetAsync(key);

            if (destStationRaw == RedisValue.Null)
            {
                return MarketResult.Fail("Unable to find commodity information for the requested sale system.");
            }

            var destStation = JsonConvert.DeserializeObject<EddnCommodityMessage>(destStationRaw);
            var commodities = destStation.Message.Commodities.Select(x => new KeyValuePair<string, Commodity>(x.Name, x));

            var destCommodities = new Dictionary<string, Commodity>(commodities);


            var possibleSourceSystems = await possibleSourceTask; // lets get that answer now, nothing else we can do until then.

            Debug.Assert(possibleSourceSystems?.Count > 0, "Search should have at least returned the sale system for a count of 1 ?!?!?!");
            var returnValue = new MarketResult() {
                Success = true,
                SystemsInRange = possibleSourceSystems.Count,
                Request = request
            }; //assume this is going to work from here in


            var tasks = new List<Task<List<ProfitableTrade>>>();
            foreach (var possibleSource in possibleSourceSystems)
            {
                tasks.Add(ShowMeTheMoney(destCommodities, possibleSource, request.MinProfit)); //get them started in parrallel
            }

            var allSystemResults = await Task.WhenAll(tasks);

            foreach (var systemResult in allSystemResults)
            {
                if (systemResult.Any())
                {
                    returnValue.Trades.AddRange(systemResult);
                }
            }

            returnValue.TimeToGenerate = timer.Elapsed;
            return returnValue;
        }

        private static async Task<List<ProfitableTrade>> ShowMeTheMoney(Dictionary<string, Commodity> destCommodities, EdsmSystemSphereResult possibleSource, int minProfit)
        {
            //Time to start some hard yakka...
            var returnValue = new List<ProfitableTrade>(); //keyed by station name

            Console.WriteLine($"Finding markets in {possibleSource.Name}...");
            var markets = await GetAllMarketsInSystem(possibleSource.Name);
            foreach (var market in markets)
            {
                var trades = ProcessMarket(market, possibleSource, minProfit, destCommodities);

                if (trades.Count() >= MIN_TRADES_PER_STATION)
                {
                    var sorted = trades.OrderByDescending(x => x.Profit);
                    returnValue.AddRange(sorted);
                }
            }

            return returnValue;
        }


        public static IEnumerable<ProfitableTrade> ProcessMarket(EbgsStation market, EdsmSystemSphereResult system, int minProfit, Dictionary<string, Commodity> destCommodities)
        {
            Console.WriteLine($"  Looking up {market.name}...");
            var marketKey = $"Market-{system.Name}-{market.name}";
            var redisData = RedisHelper.Database.StringGet(marketKey);

            if (redisData == RedisValue.Null)
            {
                Console.WriteLine("    No market data available...");
                yield break;
            }

            var marketInfo = JsonConvert.DeserializeObject<EddnCommodityMessage>(redisData);
            if (marketInfo.Header.GatewayTimestamp < DateTime.UtcNow.AddDays(-5)) //stale
            {
                Console.WriteLine("    Market data is stale...");
                yield break;
            }

            foreach (var commodity in marketInfo.Message.Commodities)
            {
                if (commodity.Stock == 0)
                {
                    Console.WriteLine($"    {commodity.Name} ignored due to no stock...");
                    continue;
                }
                else if (!destCommodities.ContainsKey(commodity.Name))
                {
                    Console.WriteLine($"    {commodity.Name} pricing at sale system not available...");
                    continue;
                }

                var profit = CalcProfit(commodity, destCommodities[commodity.Name]);
                if (profit >= minProfit)
                {
                    yield return new ProfitableTrade()
                    {
                        BuySystem = marketInfo.Message.SystemName,
                        BuyMarket = marketInfo.Message.StationName,
                        Commodity = commodity,
                        Profit = profit
                    };
                }
            }
        }


        private static int CalcProfit(Commodity buying, Commodity selling)
        {
            return selling.SellPrice - buying.BuyPrice;
        }

        public static async Task<List<EbgsStation>> GetAllMarketsInSystem(string sysName)
        {

            var markets = await RedisHelper.GetAllMarketsInSystem(sysName); //caching call
            return new List<EbgsStation>( FilterToPossibles(markets));

        }

        private static IEnumerable<EbgsStation> FilterToPossibles(IEnumerable<EbgsStation> docs)
        {
            foreach (var station in docs)
            {
                foreach (var service in station.services)
                {
                    if (service.name_lower == "commodities")
                    {
                        yield return station;
                    }
                }
            }

        }
    }
}
