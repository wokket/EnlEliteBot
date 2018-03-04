using EnlEliteBot.Web.EDDB;
using Newtonsoft.Json.Linq;
using System;

namespace EnlEliteBot.Web.EDDN
{
    public static class EDDNResultParser
    {
        internal static CmdrSavedInfo ParseJson(string result)
        {

            var json = JObject.Parse(result);

            var cmdr = (string)json.SelectToken("commander");

            var system = (string)(
                    json.SelectToken("data.systemName") ??
                    json.SelectToken("data.StarSystem"));

            var asAt = (DateTime)json.SelectToken("data.timestamp");

            var sysPosition = json.SelectToken("data.StarPos");
            var eventName = (string)json.SelectToken("data.event");

            EDDNCoordinates coOrds = null;
            if (sysPosition != null)
            {
                coOrds = new EDDNCoordinates(sysPosition.ToObject<float[]>());
            }
            else if (sysPosition == null && system != null)
            {
                //get the coOrds from EDSM
                var sysInfo = EDDBHelper.GetSystemInfoCached(system).Result; //Danger will robinson

                if (sysInfo != null)
                {
                    coOrds = new EDDNCoordinates
                    {
                        x = sysInfo.x,
                        y = sysInfo.y,
                        z = sysInfo.z
                    };
                }
            }

            return new CmdrSavedInfo
            {
                Event = eventName,
                CommanderName = cmdr,
                SystemName = system,
                LastSeenAtUtc = asAt,
                CoOrds = coOrds
            };

        }
    }
}
