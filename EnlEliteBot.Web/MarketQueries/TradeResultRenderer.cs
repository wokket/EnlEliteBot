using ConsoleTableExt;
using Flurl.Http;
using PasteSharp;
using PasteSharp.Config;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.MarketQueries
{
    public static class TradeResultRenderer
    {

        public static async Task<string> GenerateReport(MarketResult result)
        {

            if (!result.Success)
            {
                return result.FailReason;
            }




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
                .AddColumn("System", "Station", "Commodity", "Profit (cr/t)", "Stock Avail.")
                .Export();

            report.Append(table);

            var title = $"Profit Report";
            var reportUrl = await UploadToPasteBin(report.ToString(), title);

            return reportUrl;
        }

        private static async Task<string> UploadToPasteBin(string report, string title)
        {
            //we can't use a lot of baked in methods here becasue strings longer than 32kb break most of the UrlEncoders.

            var encodedReport = UrlEncoder.Default.Encode(report);

            var post = "api_option=paste&" +
                $"api_dev_key={Startup.Configuration["PasteBin.ApiKey"]}&" +
                $"api_paste_name={UrlEncoder.Default.Encode(title)}&" +
                "api_paste_expire_date=1H&" +
                "api_paste_private=1&" + //unlisted
                $"api_paste_code={encodedReport}";


            var url = "https://pastebin.com/api/api_post.php";

            using (var http = new HttpClient())
            {
                var postContent = new StringContent(post, Encoding.UTF8);
                postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await http.PostAsync(url, postContent);

                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return result;
                }

                Console.Error.WriteLine(result);
                return null;
            }

        }
    }
}
