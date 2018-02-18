using EnlEliteBot.Web;
using EnlEliteBot.Web.EDDN;
using EnlEliteBot.Web.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1
{
    public class LocationTests
    {
        private BotAsyncHandlers _handler;
        private string _msg;


        public LocationTests()
        {
            _handler = new BotAsyncHandlers((_, message) => { _msg = message; });
        }


        [Fact]
        public async Task TestLocationLookupFailure()
        {
            //This needs a working internet connection
            await _handler.SystemLookupAsync("XXX", "Test");
            Assert.Equal("EDDB has no knowledge of 'XXX'!", _msg);
        }


        [Fact]
        public async Task TestLocationLookupSuccess()
        {
            //This needs a working internet connection
            await _handler.SystemLookupAsync("? system Sol", "Test");
            Assert.Equal("https://eddb.io/system/17072", _msg);
        }

        [Fact]
        public async Task TestLocationisNOTCaseSensitive()
        {

            var cmdr = new CmdrSavedInfo
            {
                CommanderName = "UnitTest", //mixed case
                LastSeenAtUtc = DateTime.UtcNow,
                SystemName = DateTime.UtcNow.ToLongDateString()
            };
            RedisHelper.SaveData(cmdr);


            await _handler.LocateCommanderAsync("? locate unittest", "Test"); //note lower case
            var expected = $"'{cmdr.CommanderName}' last seen via EDDN in { cmdr.SystemName} at { cmdr.LastSeenAtUtc}";
            Assert.Equal(expected, _msg);
        }

    }
}
