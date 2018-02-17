using EnlEliteBot.Web.EDDN;
using EnlEliteBot.Web.Redis;

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

        private static void HandleResult(object sender, string result)
        {
            var currentState = EDDNResultParser.ParseJson(result);
            RedisHelper.SaveData(currentState);
        }
    }
}
