using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlEliteBot.Web.EDSM
{
    public class EDSMTrafficResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TrafficReport Traffic { get; set; }
    }

    public class TrafficReport
    {
        public int Total { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }


        //this is all int properties, named per ship.
        //go for a dynmic to let us only see populated properties...
        public /*TrafficBreakdown*/ dynamic Breakdown { get; set; }

    }

    public class TrafficBreakdown
    {
        public int Anaconda { get; set; }
        public int AspExplorer { get; set; }
        public int BelugaLiner { get; set; }
        public int CobraMkIII { get; set; }
        public int CobraMkIV { get; set; }
        public int DiamondbackExplorer { get; set; }
        public int DiamondbackScout { get; set; }
        public int Dolphin { get; set; }
        public int Eagle { get; set; }
        public int FederalCorvette { get; set; }
        public int FederalDropship { get; set; }
        public int FederalGunship { get; set; }
        public int Hauler { get; set; }
        public int ImperialEagle { get; set; }
        public int ImperialCourier { get; set; }
        public int ImperialCutter { get; set; }
        public int Orca { get; set; }
        public int Python { get; set; }
        public int Sidewinder { get; set; }
        public int Type10Defender { get; set; }
        public int Type7Transporter { get; set; }
        public int Type9Heavy { get; set; }
        public int ViperMkIV { get; set; }
        public int Vulture { get; set; }
    }
}
