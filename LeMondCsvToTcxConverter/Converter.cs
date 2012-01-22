using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConvertToTcx
{
    public class Converter
    {
        public void WriteTcxFile(IEnumerable<SourcedStream> laps, TextWriter textWriter)
        {
            using (TcxWriter writer = new TcxWriter(textWriter))
            {
                writer.StartTcx();
                bool firstFile = true;
                LapStats stats = new LapStats() { Calories = 0, DistanceMeters = 0, TotalTimeSeconds = 0 };
                foreach (var lap in laps)
                {
                    var data = TcxDataFactory.CreateDefault().Create(lap);
                    if (firstFile)
                    {
                        writer.StartActivity(data.StartTime, data.Sport);
                        firstFile = false;
                    }

                    writer.StartLap(data.StartTime);

                    foreach (var point in data.TrackPoints)
                    {
                        writer.StartTrackPoint();
                        writer.WriteTrackPointTime(point.Time);
                        writer.WriteTrackPointCadence(point.CadenceRpm);
                        writer.WriteTrackPointElapsedCalories(point.CaloriesElapsed + stats.Calories);
                        writer.WriteTrackPointElapsedDistanceMeters(point.DistanceMetersElapsed + stats.DistanceMeters);
                        writer.WriteTrackPointHeartRateBpm(point.HeartRateBpm);
                        writer.WriteTrackPointPowerWatts(point.PowerWatts);
                        writer.WriteTrackPointSpeedMetersPerSecond(point.SpeedMetersPerSecond);
                        writer.EndTrackPoint();
                    }

                    stats = writer.EndLap();
                }
                writer.EndActivity();
                writer.EndTcx();
            }
        }
    }
}
