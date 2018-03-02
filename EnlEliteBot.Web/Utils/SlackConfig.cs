using System.Collections.Generic;

namespace EnlEliteBot.Web.Utils
{
    public class SlackConfig
    {
        public string[] BotTokens { get; set; }
        public string[] SlackUsers { get; set; }

        public IEnumerable<KeyValuePair<string, string>> UserConfig
        {
            get
            {
                for (var i = 0; i < SlackUsers.Length; i++)
                {
                    yield return new KeyValuePair<string, string>(SlackUsers[i], BotTokens[i]);
                }
            }
        }
    }
}
