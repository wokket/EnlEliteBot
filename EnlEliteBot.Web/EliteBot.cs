using EnlEliteBot.Web.EDDB;
using Slackbot;
using System;
using System.Linq;
using System.Threading.Tasks;
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

                var task0 = EDDBHelper.GetSystemInfo(names[0]);
                var task1 = EDDBHelper.GetSystemInfo(names[1]);

                var result0 = task0.GetAwaiter().GetResult();

                if (result0.total == 0)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of '{names[0]}'!");
                    return;
                }

                var result1 = task1.GetAwaiter().GetResult();
                if (result1.total == 0)
                {
                    SendMessage(message.Channel, $"EDDB has no knowledge of '{names[1]}'!");
                    return;
                }

                var distance = CalcDistance(result0.docs[0], result1.docs[0]);

                SendMessage(message.Channel, $"Distance between {names[0]} and {names[1]} is {distance}ly");
            }
        }

        private double CalcDistance(EDDBSystemInfo sys1, EDDBSystemInfo sys2)
        {

            // see http://www.math.usm.edu/lambers/mat169/fall09/lecture17.pdf

            var xComponent = (sys2.x - sys1.x) * (sys2.x - sys1.x);
            var yComponent = (sys2.y - sys1.y) * (sys2.y - sys1.y);
            var zComponent = (sys2.z - sys1.z) * (sys2.z - sys1.z);


            var result =  Math.Sqrt(xComponent + yComponent + zComponent);
            return Math.Round(result, 2);
        }
    }
}

