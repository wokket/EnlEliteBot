using Flurl.Http;
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

    }
}
