using EnlEliteBot.Web.EDDN;
using System.Collections.Generic;
using Flurl.Http;
using EnlEliteBot.Web.Utils;
using System.Text.Encodings.Web;
using System;
using System.Threading;

namespace EnlEliteBot.Web
{
    public static class SlackHelper
    {
        private static Dictionary<string, string> _tokens;
        private static Timer _timer;
        private static Dictionary<string, DateTime> _lastUpdate = new Dictionary<string, DateTime>();

        public static void SetSlackUsers(SlackConfig data)
        {
            _tokens = new Dictionary<string, string>(data.UserConfig);
            _timer = new Timer(IdlePlayerHandler, null, 0, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);
        }

        private static void IdlePlayerHandler(object _)
        {
            // runs intermittently to detect players with no updates, and marks them offline
            foreach (var cmdr in _lastUpdate.Keys)
            {
                if (_lastUpdate[cmdr] < DateTime.Now.Subtract(TimeSpan.FromMinutes(5)))
                { // no events at all in 5 minutes
                    UpdatePlayerState(cmdr, null);
                    _lastUpdate.Remove(cmdr);
                }
            }
        }

        public static void HandleCommanderData(CmdrSavedInfo cmdr)
        {
            var cmdrName = cmdr.CommanderName.ToLower();

            if (_tokens.ContainsKey(cmdrName))
            {
                if (!string.IsNullOrEmpty(cmdr.SystemName) || cmdr.Event == "ShutDown")
                {
                    UpdatePlayerState(cmdrName, cmdr.SystemName);
                }

                _lastUpdate[cmdrName] = DateTime.Now; //connection is still active
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
