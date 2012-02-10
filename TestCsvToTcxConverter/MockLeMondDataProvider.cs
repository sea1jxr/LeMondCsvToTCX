using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConvertToTcx;

namespace TestCsvToTcxConverter
{
    class MockLeMondDataProvider : ILeMondDataProvider
    {
        public DateTime StartTime { get; set; }

        public IEnumerable<LeMondCsvDataLine> DataLines { get; set; }

        
        public virtual double ConvertSpeedToKilometersPerHour(double speed)
        {
            return speed;
        }

        public virtual double ConvertDistanceToKilometers(double distance)
        {
            return distance;
        }
    }
}
