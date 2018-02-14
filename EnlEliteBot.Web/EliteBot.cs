using Slackbot;
using System.Linq;
using static System.StringComparison;

namespace EnlEliteBot.Web
{
    public class EliteBot : Bot
    {
        private readonly BotAsyncHandlers _handlers;

        public EliteBot(string token) : base(token, "enl_elite_bot")
        {
            _handlers = new BotAsyncHandlers(SendMessage);
        }

        public void Configure()
        {
            OnMessage += PingBotHandler;
            OnMessage += SystemLookupHandler;
            OnMessage += FactionTrendHandler;
            OnMessage += DistanceHandler;
            OnMessage += LocateCommanderHandler;

        }

        #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


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
                _handlers.SystemLookupAsync(text, message.Channel);
            }
        }

        public void FactionTrendHandler(object sender, OnMessageArgs message)
        {
            var text = message.Text.ToLower();
            if (text.StartsWith("?bgs") || text.StartsWith("? bgs"))
            {
                _handlers.FactionTrendHandlerAsync(text, message.Channel);
            }
        }

        public void LocateCommanderHandler(object sender, OnMessageArgs message)
        {
            var text = message.Text.ToLower();
            if (text.StartsWith("?locate") || text.StartsWith("? locate"))
            {
                _handlers.LocateCommanderAsync(text, message.Channel);
            }
        }


        public void DistanceHandler(object sender, OnMessageArgs message)
        {

            var text = message.Text.ToLower();
            if (text.StartsWith("?distance") || text.StartsWith("? distance"))
            {
                _handlers.DistanceHandlerAsync(text, message.Channel);
            }
        }

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    }
}

