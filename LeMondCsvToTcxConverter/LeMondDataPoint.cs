using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public class LeMondDataPoint
    {
        public TimeSpan ElapsedTime { get; set; }
        public double SpeedKilometersPerHour { get; set; }
        public double DistanceKilometers { get; set; }
        public int PowerWatts { get; set; }
        public int HeartRateBeatsPerMinute { get; set; }
        public int CadenceRotationsPerMinute { get; set; }
        public int ElapsedCalories { get; set; }
    }
}
