namespace EnlEliteBot.Web.EDSM
{
    public class EDSMPlayerLocation
    {
        public string msgNum { get; set; }
        public string system { get; set; }
        public EDSMCoordinates coordinates { get; set; }
        public string dateLastActivity { get; set; }
    }

    public class EDSMCoordinates :ICoordinates
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}