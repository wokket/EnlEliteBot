using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EnlEliteBot.Web.Redis;
using Flurl;
using Flurl.Http;

namespace EnlEliteBot.Web.EDDB
{
    public static class EDDBHelper
    {
        public static async Task<EBGSSearchResult> GetSystemInfo(string systemName)
        {
            var systemNameEncoded = UrlEncoder.Default.Encode(systemName);
            var url = $"https://elitebgs.kodeblox.com/api/eddb/v3/systems?name={systemNameEncoded}";

           return await url.GetJsonAsync<EBGSSearchResult>();

        }

        internal static async Task<EDDBSystemInfo> GetSystemInfoCached(string token)
        {
            //check redis cache
            var system = RedisHelper.GetSystem(token);
            if (system != null)
            {
                return system;
            }

            var searchResult = await GetSystemInfo(token);

            if (searchResult?.total > 0)
            {
                system = searchResult.docs[0];
                RedisHelper.SaveData(system); //cache it
                return system;
            }

            return null;
        }
    }
}

