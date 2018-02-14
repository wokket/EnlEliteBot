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

        public static double CalcDistance(ICoordinates sys1, ICoordinates sys2)
        {

            // see http://www.math.usm.edu/lambers/mat169/fall09/lecture17.pdf

            var xComponent = (sys2.x - sys1.x) * (sys2.x - sys1.x);
            var yComponent = (sys2.y - sys1.y) * (sys2.y - sys1.y);
            var zComponent = (sys2.z - sys1.z) * (sys2.z - sys1.z);


            var result = Math.Sqrt(xComponent + yComponent + zComponent);
            return Math.Round(result, 2);
        }
    }
}
