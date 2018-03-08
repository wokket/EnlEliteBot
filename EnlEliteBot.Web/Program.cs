using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace EnlEliteBot.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var webhost = BuildWebHost(args);
            //EddnListener.Start(); //starts listener and returns, need to build webhost first for config
            KafkaListener.Start();
            webhost.Run(); //starts bot web host and blocks.
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5123")
                .UseStartup<Startup>()
                .Build();
    }
}
