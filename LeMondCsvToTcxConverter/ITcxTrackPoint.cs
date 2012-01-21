using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeMondCsvToTcxConverter
{
    public class TcxTrackPoint
    {
        public DateTime Time { get; set;}
        public int CadenceRpm { get; set; }
        public int ElapsedCalories { get; set; }
        public double ElapsedDistanceMeters { get; set; }
        public int HeartRateBpm { get; set; }
        public int PowerWatts { get; set; }
        public double SpeedMetersPerSecond { get; set; } 
    }
}
