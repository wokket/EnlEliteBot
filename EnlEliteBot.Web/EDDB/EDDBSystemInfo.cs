﻿using System;

namespace EnlEliteBot.Web.EDDB
{

    public class EBGSSearchResult
    {

        public EDDBSystemInfo[] docs { get; set; }
        public int total { get; set; }
        public int limit { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
    }

    public class EDDBSystemInfo : ICoordinates
    {

        public string Url { get { return $"https://eddb.io/system/{eddb_id}"; } }

        public string _id { get; set; }
        public int id { get; set; }
        public string name_lower { get; set; }
        public int eddb_id { get; set; }
        public string name { get; set; }

        public float y { get; set; }
        public float z { get; set; }
        public float x { get; set; }


    }
}
