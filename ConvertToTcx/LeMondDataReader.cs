﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public class LeMondDataReader
    {
        ILeMondDataProvider provider;

        public LeMondDataReader(ILeMondDataProvider provider)
        {
            this.provider = provider;
        }

        public DateTime StartTime
        {
            get { return provider.StartTime; }
        }

        public IEnumerable<LeMondDataPoint> DataPoints
        {
            get
            {
                foreach (var line in this.provider.DataLines)
                {
                    yield return new LeMondDataPoint()
                                    {
                                        ElapsedTime = TimeSpan.Parse(line.Time),
                                        SpeedKilometersPerHour = provider.ConvertSpeedToKilometersPerHour(double.Parse(line.Speed)),
                                        DistanceKilometers = provider.ConvertDistanceToKilometers(double.Parse(line.Distance)),
                                        PowerWatts = int.Parse(line.Power),
                                        HeartRateBeatsPerMinute = int.Parse(line.HeartRate),
                                        CadenceRotationsPerMinute = int.Parse(line.Rpm),
                                        ElapsedCalories = int.Parse(line.Calories),
                                    };
                }
            }
        }
    }

}
