using Flurl.Http;
using System;
using System.Collections.Generic;
using System.IO;
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

        internal static Task<EDSMTrafficResult> GetSystemTrafficInfo(string systemName)
        {
            var encodedName = UrlEncoder.Default.Encode(systemName);

            var url = $"https://www.edsm.net/api-system-v1/traffic?systemName={encodedName}";

            return url.GetJsonAsync<EDSMTrafficResult>();
        }

        public static async Task<List<EdsmSystemSphereResult>> GetSystemsInSphereAround(string target, int radius)
        {

            Console.WriteLine("Getting systems in sphere around target...");
            var systemName = UrlEncoder.Default.Encode(target);
            var url = $"https://www.edsm.net/api-v1/sphere-systems?showId=1&radius={radius}&systemName={systemName}";

            var rawJson = await url.GetStringAsync();
            var values = JSON.Deserialize<List<EdsmSystemSphereResult>>(rawJson);

            Console.WriteLine($"   Found {values.Count} populated systems in range...");
            return values;
        }
    }
}
