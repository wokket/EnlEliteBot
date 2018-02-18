using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDDN;
using EnlEliteBot.Web.Redis;
using StackExchange.Redis;
using System;
using Xunit;

namespace XUnitTestProject1
{
    public class RedisTests
    {
        [Fact]
        public void EnsureCmdrDataSavesAndLoads()
        {
            var toSave = new CmdrSavedInfo
            {
                CommanderName = "UnitTest",
                SystemName = "Here",
                LastSeenAtUtc = DateTime.UtcNow
            };

            RedisHelper.SaveData(toSave);

            var returnedData = RedisHelper.GetCommanderLastPosition(toSave.CommanderName);

            Assert.Equal(toSave.LastSeenAtUtc, returnedData.LastSeenAtUtc); //Ensure it's saving
        }

        [Fact]
        public void EnsureSystemDataSavesAndLoads()
        {
            var rnd = new Random();
            var toSave = new EDDBSystemInfo
            {
                id = rnd.Next(),
                name = "UnitTest"
            };

            RedisHelper.SaveData(toSave);

            var returnedData = RedisHelper.GetSystem(toSave.name);

            Assert.Equal(toSave.id, returnedData.id);
        }

        [Fact]
        public void EnsureMissReturnsNull()
        {
            var missingKey = Guid.NewGuid().ToString();

            Assert.Null(RedisHelper.GetCommanderLastPosition(missingKey));

            Assert.Null(RedisHelper.GetSystem(missingKey));

        }

        [Fact]
        public void EnsureSystemStatsAreUpdating()
        {
            //we keep stats around cache hits etc directly in redis too.

            var redis = ConnectionMultiplexer.Connect("localhost");
            var db = redis.GetDatabase();

            var sysHits = (int)db.StringGet("Stats:System Cache Hit");

            RedisHelper.GetSystem("Sol");

            var newHits = (int)db.StringGet("Stats:System Cache Hit");

            Assert.True(newHits > sysHits);

        }
    }
}
