using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeMondCsvToTcxConverter
{
    public class LeMondDataReader
    {
        private DateTime startTime;
        ILeMondDataProvider provider;

        public LeMondDataReader(ILeMondDataProvider provider)
        {
            this.provider = provider;
            string date = this.provider.StartDate;
            int year, month, day;
            if (date == null || date.Length != 6 || 
                !(
                   int.TryParse(date.Substring(0, 2), out year) &&
                   int.TryParse(date.Substring(2, 2), out month) &&
                   int.TryParse(date.Substring(4, 2), out day)
                 )
               )
            {
                throw new Exception("The start date is not in the correct format, it is expected to be in a 'YYMMDD' format");
            }
            
            // assuming 21st century....
            year += 2000;

            string time = this.provider.StartTime;
            int hour, minute;
            if (time == null || time.Length != 5 || time.IndexOf(':') != 2)
            {
                throw new Exception("The start time is not in the correct format, it is expected to be in a 'HH:MM' format");
            }

            if (!(int.TryParse(time.Substring(0, 2), out hour) && int.TryParse(time.Substring(3, 2), out minute)))
            {
                throw new Exception("The start time is not in the correct format, it is expected to be in a 'HH:MM' format where the HH and MM are integers");
            }

            startTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Local);
        }

        public DateTime StartTime
        {
            get { return startTime; }
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
                                        SpeedKilometersPerHour = double.Parse(line.Speed),
                                        DistanceKilometers = double.Parse(line.Distance),
                                        PowerWatts = int.Parse(line.Power),
                                        HeartRateBeatsPerMinute = int.Parse(line.HeartRate),
                                        CadenceRotationsPerMinute = int.Parse(line.Rpm),
                                        ElapsedCalories = int.Parse(line.Calories),
                                    };
                }
            }
        }

        private TimeSpan ConvertToTimespan(string p)
        {
            throw new NotImplementedException();
        }
    }

}
