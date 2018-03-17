using EnlEliteBot.Web.EDDN;

namespace EnlEliteBot.Web.MarketQueries
{
    public class ProfitableTrade
    {
        public string BuySystem { get; set; }
        public string BuyMarket { get; set; }
        public int Profit { get; set; }
        public Commodity Commodity { get; set; }
    }
}
