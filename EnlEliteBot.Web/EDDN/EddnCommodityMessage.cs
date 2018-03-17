using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.EDDN
{

    //these are also in the MarketStore....
    //TODO: Add a shared interfaces library

    public class EddnMessageBase<T>
    {
        [JsonProperty]
        public EddnHeader Header { get; private set; }

        [JsonProperty(PropertyName = "$schemaRef")]
        public string Schema { get; private set; }

        [JsonProperty]
        public T Message { get; set; }
    }

    public class EddnCommodityMessage : EddnMessageBase<CommodityMessage> { }

    public class CommodityMessage
    {
        [JsonProperty]
        public Commodity[] Commodities { get; private set; }
        [JsonProperty]
        public string[] Prohibited { get; private set; }
        [JsonProperty]
        public string StationName { get; private set; }
        [JsonProperty]
        public string SystemName { get; private set; }
        [JsonProperty]
        public DateTime Timestamp { get; private set; }
    }

    public class Commodity
    {
        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public int MeanPrice { get; private set; }
        [JsonProperty]
        public int BuyPrice { get; private set; }
        [JsonProperty]
        public int Stock { get; private set; }
        [JsonProperty]
        public int? StockBracket { get; private set; }
        [JsonProperty]
        public int SellPrice { get; private set; }
        [JsonProperty]
        public int Demand { get; private set; }
        [JsonProperty]
        public int? DemandBracket { get; private set; }
    }

    public class EddnHeader
    {
        [JsonProperty]
        public DateTime GatewayTimestamp { get; private set; }
        [JsonProperty]
        public string SoftwareName { get; private set; }
        [JsonProperty]
        public string SoftwareVersion { get; private set; }
        [JsonProperty]
        public string UploaderID { get; private set; }

    }
}
