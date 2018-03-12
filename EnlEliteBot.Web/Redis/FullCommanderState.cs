namespace EnlEliteBot.Web.Redis
{

    /// <summary>
    /// Clone of the ActionTracker class.  This is effectively persisted to
    /// Redis on commander state change, and we retrieve and act on it.
    /// </summary>
    public class FullCommanderState
    {
        public string CommanderName { get; set; }
        public string System { get; set; }

        //The name of the location of the player,
        //This may be a star, planet, ring, etc
        public string Body { get; set; }
        public string BodyType { get; set; }

        public string WakeState { get; set; }
        public string StationType { get; set; }
        public string StationName { get; set; }
    }
}

