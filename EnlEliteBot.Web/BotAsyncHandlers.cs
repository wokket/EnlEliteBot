using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDSM;
using EnlEliteBot.Web.MarketQueries;
using EnlEliteBot.Web.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlEliteBot.Web
{
    /// <summary>
    /// Fire and Forget handlers to run async code from the main (sync only) bot
    /// </summary>
    public class BotAsyncHandlers
    {
        public Action<string, string> SendMessage { get; }

        public BotAsyncHandlers(Action<string, string> sendMessageDelegate)
        {
            SendMessage = sendMessageDelegate;
        }

        public async Task SystemLookupAsync(string text, string channel)
        {
            var systemName = text.Replace("?system", "").Replace("? system", "").Trim();
            var result = await EDDBHelper.GetSystemInfoCached(systemName);

            if (result == null)
            {
                SendMessage(channel, $"EDDB has no knowledge of '{systemName}'!");
            }
            else
            {

                SendMessage(channel, result.Url);

            }
        }

        internal async Task FactionTrendHandlerAsync(string text, string channel)
        {
            var systemName = text.Replace("?bgs", "").Replace("? bgs", "").Trim();
            var result = await EDDBHelper.GetSystemInfoCached(systemName);

            if (result == null)
            {
                SendMessage(channel, $"EDDB has no knowledge of '{systemName}'!");
            }
            else
            {
                var url = $"https://elitebgs.kodeblox.com/system/{result._id}";
                SendMessage(channel, url);

            }
        }

        public async Task LocateCommanderAsync(string text, string channel)
        {
            var commander = text.Replace("?locate", "").Replace("? locate", "").Trim();

            var cachedPlayer = RedisHelper.GetCommanderLastPosition(commander);

            if (cachedPlayer != null)
            {
                SendMessage(channel, $"'{cachedPlayer.CommanderName}' last seen via EDDN in {cachedPlayer.SystemName} at {cachedPlayer.LastSeenAtUtc}");
                return;
            }

            var player = await EDSMHelper.GetCommanderLastPosition(commander);

            if (player == null)
            {
                SendMessage(channel, $"EDSM can't find a public commander called '{commander}'!");
            }
            else
            {
                SendMessage(channel, $"'{commander}' last seen in {player.system} at {player.dateLastActivity ?? "some point in the past"}");
            }
        }

        public async Task DistanceHandlerAsync(string text, string channel)
        {
            var systemNames = text.Replace("?distance", "").Replace("? distance", "");

            var names = systemNames.Split(':');

            if (names.Length < 2)
            {
                SendMessage(channel, "I need two things for a distance! try `? distance sys1 : sys2` ");
                return;
            }

            names[0] = names[0].Trim();
            names[1] = names[1].Trim();

            var task0 = LocationHelper.GetLocationFor(names[0]);
            var task1 = LocationHelper.GetLocationFor(names[1]);

            var result0 = await task0;

            if (result0 == null)
            {
                SendMessage(channel, $"EDDB has no knowledge of a system or public commander called '{names[0]}'!");
                return;
            }

            var result1 = await task1;
            if (result1 == null)
            {
                SendMessage(channel, $"EDDB has no knowledge of a system or public commander called '{names[1]}'!");
                return;
            }

            var distance = LocationHelper.CalcDistance(result0, result1);

            SendMessage(channel, $"Distance between {names[0]} and {names[1]} is {distance}ly");
        }

        internal async Task BGSProfitHandler(string text, string channel)
        {
            text = text.Replace("? bgsprofit", "");
            var tokens = text.Split(':');

            var system = tokens[0].Trim();
            var market = tokens[1].Trim();

            try
            {
                var request = new BGSProfitRequest(system, market);
                var result = await MarketQuery.GetPurchaseStation(request);
                var reportUrl = await TradeResultRenderer.GenerateReport(result);

                SendMessage(channel, reportUrl);
            }
            catch (Exception ex)
            {
                SendMessage(channel, $"Profit generation failed: {ex.ToString()}");
            }
        }

        internal async Task TrafficHandlerAsync(string text, string channel)
        {
            var systemName = text.Replace("?traffic", "").Replace("? traffic", "").Trim();
            var result = await EDSMHelper.GetSystemTrafficInfo(systemName);

            if (result?.Id == 0)
            {
                SendMessage(channel, $"EDSM has no knowledge of '{systemName}'!");
            }
            else
            {
                var newLine = "\n";
                var message = $"Traffic Report for {result.Name}:" +
                    $"Today: {result.Traffic.Day}     This Week: {result.Traffic.Week} {newLine}{newLine}" +
                    $"Breakdown: {newLine}";

                //Breakdown is only populated with ship types that have values
                // use the dynamic stuff to only include those props in the output
                foreach (var key in result.Breakdown.Keys)
                {
                    message += $"{key}: {result.Breakdown[key]} {newLine}";
                }

                SendMessage(channel, message);
            }
        }
    }
}
