using Flurl.Http;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.EDSM
{
    public static class EDSMHelper
    {

        public static async Task<SystemInfo> GetSystemInfo(string systemName)
        {
            var systemNameEncoded = UrlEncoder.Default.Encode(systemName);
            var url = $"https://www.edsm.net/api-v1/system?showId=1&systemName={systemNameEncoded}";

            var json = await url.GetStringAsync();

            //EDSM returns an empty _array_ for no result, but a single _object_ if found... this destroys our ability to have a single return type (either T, or List<T>)
            if (json.StartsWith('['))
            {
                return null;
            }

            return JSON.Deserialize<SystemInfo>(json);

        }

        public static async Task<EDSMPlayerLocation> GetCommanderLastPosition(string playerName)
        {
            var name = UrlEncoder.Default.Encode(playerName.Trim());
            var url = $"https://www.edsm.net/api-logs-v1/get-position?commanderName={name}&showCoordinates=1";

            var json = await url.GetStringAsync();


            //EDSM returns an empty _array_ for no result, but a single _object_ if found... this destroys our ability to have a single return type (either T, or List<T>)
            if (json.StartsWith('[') || json.StartsWith("{\"msgnum\":203"))
            {
                return null;
            }

            return JSON.Deserialize<EDSMPlayerLocation>(json);
        }

        internal static async Task<EDSMTrafficResult> GetSystemTrafficInfo(string systemName)
        {
            var encodedName = UrlEncoder.Default.Encode(systemName);

            var url = $"https://www.edsm.net/api-system-v1/traffic?systemName={encodedName}";

            var result = await url.GetJsonAsync<EDSMTrafficResult>();

            return result;
        }
    }
}
