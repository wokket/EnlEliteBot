using EnlEliteBot.Web.EDDN;
using System.Collections.Generic;
using Flurl.Http;
using EnlEliteBot.Web.Utils;
using System.Text.Encodings.Web;
using System;
using System.Linq;
using System.Threading;
using EnlEliteBot.Web.Redis;

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
            var keys = _lastUpdate.Keys.ToArray(); //prevent modified ienumerable exception
            foreach (var cmdr in keys)
            {
                if (_lastUpdate[cmdr] < DateTime.Now.Subtract(TimeSpan.FromMinutes(15)))
                { // no status updates at all in 15 minutes
                    UpdateSlack(cmdr, new SlackStatus()); //blank status
                    _lastUpdate.Remove(cmdr);
                }
            }
        }

        //public static void HandleCommanderData(CmdrSavedInfo cmdr)
        //{
        //    var cmdrName = cmdr.CommanderName.ToLower();

        //    if (_tokens.ContainsKey(cmdrName))
        //    {
        //        if (!string.IsNullOrEmpty(cmdr.SystemName) || cmdr.Event == "ShutDown")
        //        {
        //            UpdatePlayerState(cmdrName, cmdr.SystemName);
        //        }

        //        _lastUpdate[cmdrName] = DateTime.Now; //connection is still active
        //    }
        //}


        public static void HandleCommanderData(string commanderName, FullCommanderState cmdr)
        {
            var cmdrName = commanderName.ToLower();

            if (_tokens.ContainsKey(cmdrName))
            {
                UpdatePlayerState(cmdrName, cmdr);
                _lastUpdate[cmdrName] = DateTime.Now; //connection is still active
            }
        }

        private static void UpdatePlayerState(string cmdrName, FullCommanderState state)
        {
            var payload = new SlackStatus();

            if (state == null || state.WakeState == "LoggedOut") // clear the status
            {
                UpdateSlack(cmdrName, payload);
                return;
            }

            switch (state.WakeState)
            {
                case "Docked":
                    payload.status_emoji = $":{state.StationType.ToLower()}:";
                    break;

                case "Normal Space":
                    payload.status_emoji = $":airplane:";
                    break;

                case "Supercruise":
                    payload.status_emoji = $":rocket:";
                    break;

                case "Witchspace":
                    payload.status_emoji = $":witch:";
                    break;
                default:
                    payload.status_emoji = "";
                    break;
            }

            payload.status_text = $"{state.WakeState} in {state.System}";

            if (!string.IsNullOrEmpty(state.StationName))
            {
                payload.status_text += $" at {state.StationName}";
            }
            else if (!string.IsNullOrEmpty(state.Body))
            {
                payload.status_text += $" near {state.Body}";
            }

            UpdateSlack(cmdrName, payload);
            //Console.WriteLine($"{payload.status_emoji} /// {payload.status_text}");
        }

        //private static void UpdatePlayerState(string cmdrName, string systemName)
        //{
        //    var payload = new SlackStatus();

        //    if (systemName != null)
        //    {
        //        payload.status_text = $"In {systemName}";
        //        payload.status_emoji = ":space_invader:";
        //    }
        //    else
        //    {
        //        payload.status_text = "";
        //        payload.status_emoji = "";
        //    };


        //    //UpdateSlack(cmdrName, payload);
        //}

        private static void UpdateSlack(string cmdrName, SlackStatus payload)
        {
            var payloadJson = UrlEncoder.Default.Encode(JSON.Serialize(payload));
            var url = $"https://slack.com/api/users.profile.set?profile={payloadJson}&token={_tokens[cmdrName]}";

            var result = url.GetAsync().Result; //danger!!

            var response = result.Content.ReadAsStringAsync().Result;
            //Console.Out.WriteLine($"*** Slack status response: {response}");
        }

        private class SlackStatus
        {
            public SlackStatus()
            {
                status_emoji = "";
                status_text = "";
            }
            public string status_text { get; set; }
            public string status_emoji { get; set; }
        }
    }
}
