using ConsoleTableExt;
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
            var report = new StringBuilder();

            report.AppendLine("Profitable Trade Request");
            report.AppendLine("========================");
            report.AppendLine();
            report.AppendLine($"Looking for multiple profitable trades to {result.Request.SaleSystem} - {result.Request.SaleMarket}");
            report.AppendLine($"Radius for search: {result.Request.RadiusLY}ly");
            report.AppendLine();
            report.AppendLine($"Number of systems in radius: {result.SystemsInRange}");
            report.AppendLine($"Time to generate: {result.TimeToGenerate}");
            report.AppendLine();
            report.AppendLine();

            var tableData = result.Trades.Select(x => new object[] { x.BuySystem, x.BuyMarket, x.Commodity.Name, x.Profit, x.Commodity.Stock });
            var table = ConsoleTableBuilder.From(new List<object[]>(tableData))
                .AddColumn("System", "Station", "Commodity", "Profit (cr/t", "Stock")
                .Export();

            report.Append(table);




            return report.ToString();
        }
    }
}
