using System;

namespace EnlEliteBot.Web.EDDN
{
    /// <summary>
    /// This is persisted in Redis
    /// </summary>
    public class CmdrSavedInfo
    {
        public string CommanderName { get; set; }
        public string SystemName { get; set; }
        public DateTime LastSeenAtUtc { get; set; }

        public EDDNCoordinates CoOrds { get; set; }
    }

    public class EDDNCoordinates : ICoordinates
    {

        public EDDNCoordinates()
        {

        }

        public EDDNCoordinates(float[] v)
        {
            x = v[0];
            y = v[1];
            z = v[2];
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
