using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public interface ILeMondDataProvider
    {
        DateTime StartTime { get; }
        IEnumerable<LeMondCsvDataLine> DataLines { get; }

        double ConvertSpeedToKilometersPerHour(double speed);
        double ConvertDistanceToKilometers(double distance);

    }
}
