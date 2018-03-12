using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDDN;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Reflection;

namespace EnlEliteBot.Web.Redis
{
    public static class RedisHelper
    {
        private static readonly ConnectionMultiplexer _redis;
        private static readonly IDatabase _db;
        private static readonly ISubscriber _subscriber;

        static RedisHelper()
        {
            _redis = ConnectionMultiplexer.Connect("localhost");
            _db = _redis.GetDatabase();
            _subscriber = _redis.GetSubscriber();
        }

        public static void ConfigureSubscription()
        {
            //The ActionTracker manages tracking the full state of a commander at any point in time, and writes that info to redis under the "Status:{commanderName}" key.
            //Subscribe to changes in those values so we can update the slack message as appropriate.
            _subscriber.Subscribe("__keyspace@0__*", HandleCommanderStatusChanged);
            var result = _subscriber.Ping();
        }

        private static void HandleCommanderStatusChanged(RedisChannel channel, RedisValue value)
        {
            var commander = ((string)channel).Replace("__keyspace@0__:Status:", "");
            var rawData = _db.HashGetAll($"Status:{commander}");

            var data = rawData.ConvertFromRedis<FullCommanderState>();
            SlackHelper.HandleCommanderData(commander, data);
        }

        public static CmdrSavedInfo GetCommanderLastPosition(string commanderName)
        {
            var data = _db.StringGet("CMDR:" + commanderName.ToLower());

            if (data == RedisValue.Null)
            {
                _db.StringIncrement("Stats:Cmdr Cache Miss");
                return null; //not found
            }

            _db.StringIncrement("Stats:Cmdr Cache Hit", flags: CommandFlags.FireAndForget);
            return JsonConvert.DeserializeObject<CmdrSavedInfo>(data);
        }

        public static void SaveData(CmdrSavedInfo currentState)
        {
            var toSave = JsonConvert.SerializeObject(currentState);
            Console.WriteLine($"{currentState.CommanderName}: {toSave}");

            var key = "CMDR:" + currentState.CommanderName.ToLower();
            _db.StringSet(key, toSave);
        }

        public static void SaveData(EDDBSystemInfo data)
        {
            var toSave = JsonConvert.SerializeObject(data);
            var key = "SYS:" + data.name;
            _db.StringSet(key, toSave);
        }

        public static EDDBSystemInfo GetSystem(string sysName)
        {
            var data = _db.StringGet("SYS:" + sysName);

            if (data == RedisValue.Null)
            {
                Console.WriteLine($"Redis sys cache miss for {sysName}");
                _db.StringIncrement("Stats:System Cache Miss", flags: CommandFlags.FireAndForget);

                return null; //not found
            }

            _db.StringIncrement("Stats:System Cache Hit");
            return JsonConvert.DeserializeObject<EDDBSystemInfo>(data);
        }



        //Deserialize from Redis format
        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }
    }

}

