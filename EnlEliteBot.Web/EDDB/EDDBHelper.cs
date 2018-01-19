using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
    }
}

