using System.Collections.Generic;

namespace EnlEliteBot.Web.MarketQueries
{
    public class MarketResult
    {

        public MarketResult()
        {
            Trades = new List<Dictionary<string, List<ProfitableTrade>>>();
        }

        public bool Success { get; set; }
        public string FailReason { get; set; }

        public int SystemsInRange { get; set; }
        public List<Dictionary<string, List<ProfitableTrade>>> Trades { get; internal set; }

        public static MarketResult Fail(string reason)
        {
            return new MarketResult
            {
                Success = false,
                FailReason = reason
            };
        }


    }
}
