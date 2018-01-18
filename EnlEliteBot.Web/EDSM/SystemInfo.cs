using System.Text.Encodings.Web;

namespace EnlEliteBot.Web.EDSM
{
    public sealed class SystemInfo
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long Id64 { get; set; }


        public string Url
        {
            get
            {
                var encodedName = UrlEncoder.Default.Encode(Name);
                return $"https://www.edsm.net/en/system/id/{Id}/name/{encodedName}";
            }
        }
    }
}
