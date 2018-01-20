using EnlEliteBot.Web.EDDB;
using Slackbot;
using System.Linq;
using static System.StringComparison;

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

            var text = message.Text.ToLower();
            if (text.StartsWith("?system", OrdinalIgnoreCase) ||
                text.StartsWith("? system", OrdinalIgnoreCase))
            {


                var systemName = text.Replace("?system", "").Replace ("? system", "").Trim();
                var result = EDDBHelper.GetSystemInfo(systemName).Result;

                if (result == null || result.total == 0)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of '{systemName}'!");
                }
                else
                {
                    foreach (var system in result.docs)
                    {
                        SendMessage(message.Channel, system.Url);
                    }
                }
            }
        }


    }
}
