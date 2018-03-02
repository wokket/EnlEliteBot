using EnlEliteBot.Web.EDDN;
using System.Collections.Generic;
using Flurl.Http;
using EnlEliteBot.Web.Utils;
using System.Text.Encodings.Web;

namespace EnlEliteBot.Web
{
    public static class SlackHelper
    {
        private static Dictionary<string, string> _tokens;

        public static void SetSlackUsers(SlackConfig data)
        {
            _tokens = new Dictionary<string, string>(data.UserConfig);
        }

        public static void HandleCommanderData(CmdrSavedInfo cmdr)
        {
            var cmdrName = cmdr.CommanderName.ToLower();

            if (_tokens.ContainsKey(cmdrName))
            {
                UpdatePlayerState(cmdrName, cmdr.SystemName);
            }
        }

        public static void UpdatePlayerState(string cmdrName, string systemName)
        {
            var payload = new SlackStatus();

            if (systemName != null)
            {
                payload.status_text = $"In {systemName}";
                payload.status_emoji = ":space_invader:";
            }
            else
            {
                payload.status_text = "";
                payload.status_emoji = "";
            };

            var payloadJson = UrlEncoder.Default.Encode(JSON.Serialize(payload));
            var url = $"https://slack.com/api/users.profile.set?profile={payloadJson}&token={_tokens[cmdrName]}";

            var result = url.GetAsync().Result; //danger!!

            var response = result.Content.ReadAsStringAsync().Result;
        }



        private class SlackStatus
        {
            public string status_text { get; set; }
            public string status_emoji { get; set; }
        }
    }
}
