using System;
using System.Collections.Generic;

namespace EnlEliteBot.Web.MarketQueries
{
    public class MarketResult
    {

        public MarketResult()
        {
            Trades = new List<ProfitableTrade>();
        }

        public bool Success { get; set; }
        public string FailReason { get; set; }

        public int SystemsInRange { get; set; }
        public List<ProfitableTrade> Trades { get; internal set; }

        public TimeSpan TimeToGenerate { get; set; }
        public BGSProfitRequest Request { get; internal set; }

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
