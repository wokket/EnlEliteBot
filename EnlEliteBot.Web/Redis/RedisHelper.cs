using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDDN;
using EnlEliteBot.Web.EDSM;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace EnlEliteBot.Web.Redis
{
    public static class RedisHelper
    {
        private static readonly ConnectionMultiplexer _redis;
        private static readonly IDatabase _db;

        static RedisHelper()
        {
            _redis = ConnectionMultiplexer.Connect("localhost");
            _db = _redis.GetDatabase();
        }

        internal static CmdrSavedInfo GetCommanderLastPosition(string commanderName)
        {
            var data = _db.StringGet("CMDR:" + commanderName);

            if (data == RedisValue.Null)
            {
                return null; //not found
            }

            return JsonConvert.DeserializeObject<CmdrSavedInfo>(data);
        }

        internal static void SaveData(CmdrSavedInfo currentState)
        {

            var toSave = JsonConvert.SerializeObject(currentState);
            Console.WriteLine($"{currentState.CommanderName}: {toSave}");

            var key = "CMDR:" + currentState.CommanderName;
            _db.StringSet(key, toSave);
        }

        internal static void SaveData(EDDBSystemInfo data)
        {
            var toSave = JsonConvert.SerializeObject(data);
            var key = "SYS:" + data.name;
            _db.StringSet(key, toSave);
        }

        internal static EDDBSystemInfo GetSystem(string sysName)
        {
            var data = _db.StringGet("SYS:" + sysName);

            if (data == RedisValue.Null)
            {
                Console.WriteLine($"Redis sys cache miss for {sysName}");
                return null; //not found
            }

            Console.WriteLine("Redis sys cacche hit");
            return JsonConvert.DeserializeObject<EDDBSystemInfo>(data);
        }

    }
}
