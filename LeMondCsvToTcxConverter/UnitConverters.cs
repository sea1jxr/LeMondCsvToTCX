using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public static class ConvertDistance
    {
        const int MetersPerKilometer = 1000;
        const double KilometersPerMile = 1.609344;
        public static double KilometersToMeters(double kilometers)
        {
            return kilometers * MetersPerKilometer;
        }

        internal static double MilesToKilometers(double miles)
        {
            return miles * KilometersPerMile;
        }
    }

    public static class ConvertTime
    {
        const double HoursPerSecond = 1.0 / (60.0 * 60.0);
        public static double SecondsToHours(double seconds)
        {
            return seconds * HoursPerSecond;
        }
    }
}
