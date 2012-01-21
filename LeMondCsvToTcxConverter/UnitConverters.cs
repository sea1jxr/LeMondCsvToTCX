using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeMondCsvToTcxConverter
{
    public static class ConvertMetric
    {
        const int MetersPerKilometer = 1000;
        public static double KilometersToMeters(double kilometers)
        {
            return kilometers * MetersPerKilometer;
        }
    }

    public static class ConvertTime
    {
        const double HoursPerSecond = 1.0 / (60.0 * 60.0);
        public static double HoursToSeconds(double hours)
        {
            return hours * HoursPerSecond;
        }
    }
}
