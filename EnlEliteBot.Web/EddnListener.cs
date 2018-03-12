using EnlEliteBot.Web.EDDN;
using EnlEliteBot.Web.Redis;
using System;
using System.Linq;

namespace EnlEliteBot.Web
{

    /// <summary>
    /// Consumes the EDDN ZermoMQ feed, and saves the most recent location of a cmdr away in redis.
    /// This can then be read by the EliteBot to locate commanders that don't have a public EDSM profile.
    /// </summary>
    public static class EddnListener
    {
        public static void Start()
        {
            var client = new EddnClient();
            client.OnMessageReceived += HandleResult;
            client.StartOnBackgroundThread();
        }

        public static void HandleResult(object sender, string result)
        {
            try
            {
                var currentState = EDDNResultParser.ParseJson(result);
                if (currentState.SystemName != null)
                {
                    RedisHelper.SaveData(currentState);
                }
            }
            catch (Exception ex) //ensure a data issue on a single record doesn't blow us apart.
            {
                Console.WriteLine("Error handling EDDN data:");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Data being parsed: " + result);
            }
        }
    }
}
