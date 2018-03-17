using System;

namespace EnlEliteBot.Web.EDDB
{

    public class EbgsStationResult
    {
        public EbgsStation[] docs { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
    }

    public class EbgsStation
    {
        public string _id { get; set; }
        public string name_lower { get; set; }
        public string system_lower { get; set; }
        public int __v { get; set; }
        public string name { get; set; }
        public string system { get; set; }
        public string type { get; set; }
        public string government { get; set; }
        public string economy { get; set; }
        public string allegiance { get; set; }
        public string state { get; set; }
        public float distance_from_star { get; set; }
        public string controlling_minor_faction { get; set; }
        public Service[] services { get; set; }
        public DateTime updated_at { get; set; }
        public int eddb_id { get; set; }
    }

    public class Service
    {
        public string name_lower { get; set; }
        public string name { get; set; }
    }

}
