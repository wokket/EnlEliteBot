using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDSM;
using Slackbot;
using System;
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
            OnMessage += FactionTrendHandler;
            OnMessage += DistanceHandler;
            OnMessage += LocateCommanderHandler;
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


                var systemName = text.Replace("?system", "").Replace("? system", "").Trim();
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

        public void FactionTrendHandler(object sender, OnMessageArgs message)
        {
            var text = message.Text.ToLower();
            if (text.StartsWith("?bgs") || text.StartsWith("? bgs"))
            {
                var systemName = text.Replace("?bgs", "").Replace("? bgs", "").Trim();
                var result = EDDBHelper.GetSystemInfo(systemName).Result;

                if (result == null || result.total == 0)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of '{systemName}'!");
                }
                else
                {
                    foreach (var system in result.docs)
                    {
                        var url = $"https://elitebgs.kodeblox.com/system/{system._id}";
                        SendMessage(message.Channel, url);
                    }
                }

            }
        }

        public void LocateCommanderHandler(object sender, OnMessageArgs message)
        {
            var text = message.Text.ToLower();
            if (text.StartsWith("?locate") || text.StartsWith("? locate"))
            {
                var commander = text.Replace("?locate", "").Replace("? locate", "");

                var player = EDSMHelper.GetCommanderLastPosition(commander).Result;

                if (player == null)
                {
                    SendMessage(message.Channel, $"EDSM can't find a public commander called '{commander}'!");
                }
                else
                {
                    SendMessage(message.Channel, $"'{commander}' last seen in {player.system}");
                }
            }
        }


        public void DistanceHandler(object sender, OnMessageArgs message)
        {

            var text = message.Text.ToLower();
            if (text.StartsWith("?distance") || text.StartsWith("? distance"))
            {
                var systemNames = text.Replace("?distance", "").Replace("? distance", "");

                var names = systemNames.Split(':');

                if (names.Length < 2)
                {
                    SendMessage(message.Channel, "I need two things for a distance! try `? distance sys1 : sys2` ");
                    return;
                }

                names[0] = names[0].Trim();
                names[1] = names[1].Trim();

                var task0 = LocationHelper.GetLocationFor(names[0]);
                var task1 = LocationHelper.GetLocationFor(names[1]);

                var result0 = task0.GetAwaiter().GetResult();

                if (result0 == null)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of a system or public commander called '{names[0]}'!");
                    return;
                }

                var result1 = task1.GetAwaiter().GetResult();
                if (result1 == null)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of a system or public commander called '{names[1]}'!");
                    return;
                }

                var distance = LocationHelper.CalcDistance(result0, result1);

                SendMessage(message.Channel, $"Distance between {names[0]} and {names[1]} is {distance}ly");
            }
        }


    }
}

