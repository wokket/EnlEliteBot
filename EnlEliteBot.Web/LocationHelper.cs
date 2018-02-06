using EnlEliteBot.Web.EDDB;
using EnlEliteBot.Web.EDSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlEliteBot.Web
{
    public static class LocationHelper
    {

        /// <summary>
        /// token may be a system name, or a player name
        /// return null if not found
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<ICoordinates> GetLocationFor(string token)
        {
            var system = await EDDBHelper.GetSystemInfo(token);

            if (system?.total > 0)
            {
                return system.docs[0];
            }

            //not a system, try for a commander

            var player = await EDSMHelper.GetCommanderLastPosition(token);
            return player?.coordinates; //the coords, or null
        }
    }
}
