using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDDN;
using Flurl.Http;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.Redis
{
    public static class RedisHelper
    {
        private static readonly ConnectionMultiplexer _redis;
        public static readonly IDatabase Database;
        private static readonly ISubscriber _subscriber;

        static RedisHelper()
        {
            _redis = ConnectionMultiplexer.Connect("localhost");
            Database = _redis.GetDatabase();
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
            Console.Out.WriteLine($"REDIS NOTIFICATION: {channel} /// {value}");
            var commander = ((string)channel).Replace("__keyspace@0__:Status:", "");
            var rawData = Database.HashGetAll($"Status:{commander}");

            var data = rawData.ConvertFromRedis<FullCommanderState>();
            SlackHelper.HandleCommanderData(commander, data);
        }

        public static CmdrSavedInfo GetCommanderLastPosition(string commanderName)
        {
            var data = Database.StringGet("CMDR:" + commanderName.ToLower());

            if (data == RedisValue.Null)
            {
                Database.StringIncrement("Stats:Cmdr Cache Miss", flags: CommandFlags.FireAndForget);
                return null; //not found
            }

            Database.StringIncrement("Stats:Cmdr Cache Hit", flags: CommandFlags.FireAndForget);
            return JsonConvert.DeserializeObject<CmdrSavedInfo>(data);
        }

        public static void SaveData(CmdrSavedInfo currentState)
        {
            var toSave = JsonConvert.SerializeObject(currentState);
            Console.WriteLine($"{currentState.CommanderName}: {toSave}");

            var key = "CMDR:" + currentState.CommanderName.ToLower();
            Database.StringSet(key, toSave);
        }

        public static void SaveData(EDDBSystemInfo data)
        {
            var toSave = JsonConvert.SerializeObject(data);
            var key = "SYS:" + data.name;
            Database.StringSet(key, toSave);
        }

        public static EDDBSystemInfo GetSystem(string sysName)
        {
            var data = Database.StringGet("SYS:" + sysName);

            if (data == RedisValue.Null)
            {
                Console.WriteLine($"Redis sys cache miss for {sysName}");
                Database.StringIncrement("Stats:System Cache Miss", flags: CommandFlags.FireAndForget);

                return null; //not found
            }

            Database.StringIncrement("Stats:System Cache Hit", flags: CommandFlags.FireAndForget);
            return JsonConvert.DeserializeObject<EDDBSystemInfo>(data);
        }

        public static async Task<List<EbgsStation>> GetAllMarketsInSystem(string sysName)
        {

            var key = $"STATIONS:{sysName}";
            var data = await Database.StringGetAsync(key);

            if (data == RedisValue.Null)
            {
                Database.StringIncrement("Stats:MarketsInSystem Cache Miss", flags: CommandFlags.FireAndForget);
                var sourceData = await GetAllMarketsInSystemFromSource(sysName);

                //non-blocking save
                Database.StringSet(key, JsonConvert.SerializeObject(sourceData), flags: CommandFlags.FireAndForget);
                return sourceData;
            }

            Database.StringIncrement("Stats:MarketsInSystem Cache Hit", flags: CommandFlags.FireAndForget);
            return JsonConvert.DeserializeObject<List<EbgsStation>>(data);
        }

        private static async Task<List<EbgsStation>> GetAllMarketsInSystemFromSource(string sysName)
        {
            var encodedSysName = UrlEncoder.Default.Encode(sysName);
            var url = $"https://elitebgs.kodeblox.com/api/ebgs/v4/stations?system={encodedSysName}";

            var result = await url.GetJsonAsync<EbgsStationResult>();

            var stationsToReturn = new List<EbgsStation>();

            stationsToReturn.AddRange(result.docs);

            while (result.page < result.pages)
            {
                var newUrl = url + $"&page={result.page + 1}"; //get next page of results
                result = await newUrl.GetJsonAsync<EbgsStationResult>();
                stationsToReturn.AddRange(result.docs);
            }

            return stationsToReturn;
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

