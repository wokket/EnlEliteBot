using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDSM;
using Slackbot;
using System.Linq;
using System.Text.Encodings.Web;

namespace EnlEliteBot.Web
{
    public class EliteBot : Bot
    {
        public EliteBot(string token) : base(token, "enl_elite_bot")
        {
        }

        public void Configure()
        {
            OnMessage += PingBotHandler;
            OnMessage += SystemLookupHandler;
        }


        public void PingBotHandler(object sender, OnMessageArgs message)
        {
            if (message.MentionedUsers.Any(user => user == "enl_elite_bot"))
            {
                SendMessage(message.Channel, $"Hi {message.User}, thanks for thinking of me!");
            }
        }

        public void SystemLookupHandler(object sender, OnMessageArgs message)
        {
            if (message.Text.StartsWith("?system"))
            {
                var systemName = message.Text.Replace("?system", "").Trim();
                var result = EDDBHelper.GetSystemInfo(systemName).Result;

                if (result == null)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of '{systemName}'!");
                }
                else
                {
                    SendMessage(message.Channel, result.Url);
                }
            }
        }


    }
}
