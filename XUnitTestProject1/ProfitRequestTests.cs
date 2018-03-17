using EnlEliteBot.Web.EDSM;
using EnlEliteBot.Web.MarketQueries;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1
{
    public class ProfitRequestTests
    {

        [Fact]
        public async Task TestFindingProfit()
        {
            var request = new BGSProfitRequest("Ra", "LeConte Dock", 20);

            var result = await MarketQuery.GetPurchaseStation(request);

            // took 40sec naiive sequential run, 7sec parallelised, 3.5sec with market caching in redis
            Assert.True(result.Success);
            Assert.True(result.Trades.Count > 0);

            var output = TradeResultRenderer.GenerateReport(result);


        }


        [Fact]
        public async Task TestBadSaleStationInfo()
        {
            var request = new BGSProfitRequest("Unknown", "I Dont Exist", 2);
            var result = await MarketQuery.GetPurchaseStation(request);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task TestSystemSphereLookup()
        {
            var results = await EDSMHelper.GetSystemsInSphereAround("Sol", 5);
            Assert.True(results.Count == 2); //sol and alpha centuri
        }

        [Fact]
        public async Task TestSystemSphereLookupAlwaysReturnsSource()
        {
            var results = await EDSMHelper.GetSystemsInSphereAround("Sol", 1);
            Assert.True(results.Count == 1); //sol itself
        }

        [Fact]
        public async Task TestGetMarketsInSystem()
        {
            var result = await MarketQuery.GetAllMarketsInSystem("Ra");
            Assert.True(result?.Count > 0);

        }
    }
}