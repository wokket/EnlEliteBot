using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.MarketQueries
{
    public static class TradeResultRenderer
    {

        public static string GenerateReport(MarketResult result)
        {
            var stringBuilder = new StringBuilder();

            foreach(var item in result.Trades)
            {
                stringBuilder.AppendLine($"{item.BuySystem} ({item.BuyMarket}) - {item.Commodity.Name} for {item.Profit}cr/t profit.  Supply: {item.Commodity.Stock}");
            }

            return stringBuilder.ToString();
        }
    }
}
