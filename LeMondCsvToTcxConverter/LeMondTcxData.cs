using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public class LeMondTcxData : ITcxData
    {
        static readonly TimeSpan oneSecond = new TimeSpan(0, 0, 1);

        private LeMondDataReader reader;
        public LeMondTcxData(LeMondDataReader reader)
        {
            this.reader = reader;
        }

        public DateTime StartTime
        {
            get { return reader.StartTime; }
        }

        public TcxSport Sport
        {
            get { return TcxSport.Biking; }
        }

        public IEnumerable<TcxTrackPoint> TrackPoints
        {
            get 
            {
                bool firstPoint = true;
                foreach (var point in reader.DataPoints)
                {
                    if (firstPoint)
                    {
                        // adding a fake first point becuase strava seems
                        // to like seeing seconds 0:00-1:00 for a minute instead of 0:01-1:00
                        // this new point will actually give us 61 points, but will be considerd
                        // a full minute
                        yield return CreateTrackPoint(point.ElapsedTime - oneSecond, point);
                        firstPoint = false;
                    }
                    yield return CreateTrackPoint(point.ElapsedTime, point);
                }

            }
        }

        private TcxTrackPoint CreateTrackPoint(TimeSpan effectiveElapsedTime, LeMondDataPoint point)
        {
            return new TcxTrackPoint()
            {
                Time = reader.StartTime + effectiveElapsedTime,
                CadenceRpm = point.CadenceRotationsPerMinute,
                CaloriesElapsed = point.ElapsedCalories,
                DistanceMetersElapsed = ConvertDistance.KilometersToMeters(point.DistanceKilometers),
                HeartRateBpm = point.HeartRateBeatsPerMinute,
                PowerWatts = point.PowerWatts,
                SpeedMetersPerSecond = ConvertTime.SecondsToHours(ConvertDistance.KilometersToMeters(point.SpeedKilometersPerHour)),
            };
        }
    }
}
