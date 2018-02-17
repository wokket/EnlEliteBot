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

            var cmdr = (string)json.SelectToken("header.uploaderID");

            var system = (string)(
                    json.SelectToken("message.systemName") ??
                    json.SelectToken("message.StarSystem"));

            var asAt = (DateTime)json.SelectToken("message.timestamp");

            var sysPosition = json.SelectToken("message.StarPos");

            EDDNCoordinates coOrds = null;
            if (sysPosition != null)
            {
                coOrds = new EDDNCoordinates(sysPosition.ToObject<float[]>());
            }
            else if (sysPosition == null && system != null)
            {
                //get the coOrds from EDSM
                var sysInfo = EDDBHelper.GetSystemInfoCached(system).Result; //Danger will robinson

                coOrds = new EDDNCoordinates
                {
                    x = sysInfo.x,
                    y = sysInfo.y,
                    z = sysInfo.z
                };
            }

            return new CmdrSavedInfo
            {
                CommanderName = cmdr,
                SystemName = system,
                LastSeenAtUtc = asAt,
                CoOrds = coOrds
            };

        }
    }
}
