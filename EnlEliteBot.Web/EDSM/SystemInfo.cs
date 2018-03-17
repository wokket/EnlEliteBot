using System.Diagnostics;
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

    [DebuggerDisplay("EdsmSystemSphereResult ({Id}:{Name})")]
    public class EdsmSystemSphereResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CoOrds Coords { get; set; }
        public float? Distance { get; set; }

    }

    public class CoOrds
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
