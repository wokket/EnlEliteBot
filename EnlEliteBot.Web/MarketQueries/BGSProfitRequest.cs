namespace EnlEliteBot.Web.MarketQueries
{
    /// <summary>
    /// Class to represent a request for a "BGS Profitable" station.
    /// ie. multiple good profitable >= 750cr/t
    /// </summary>
    public class BGSProfitRequest
    {

        public BGSProfitRequest(string sysName, string marketName, byte radius = 50, int minProfit = 750)
        {
            RadiusLY = radius;
            MinProfit = minProfit;

            SaleSystem = sysName;
            SaleMarket = marketName;
        }


        public string SaleSystem { get; set; }
        public string SaleMarket { get; set; }
        public byte RadiusLY { get; set; }
        public int MinProfit { get; set; }
    }
}
