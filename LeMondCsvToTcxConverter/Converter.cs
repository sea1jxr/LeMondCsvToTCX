using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LeMondCsvToTcxConverter
{
    public class Converter
    {
        const int MetersPerKilometer = 1000;
        const double HoursPerSecond = 1.0 / (60.0 * 60.0);
        public void WriteTcxFile(IEnumerable<TextReader> laps, TextWriter textWriter)
        {
            TimeSpan oneSecond = new TimeSpan(0, 0, 1);
            using (TcxWriter writer = new TcxWriter(textWriter))
            {
                writer.StartTcx();
                bool firstFile = true;
                LapStats stats = new LapStats() { Calories = 0, DistanceMeters = 0, TotalTimeSeconds = 0 };
                foreach (var lap in laps)
                {
                    var provider = new LeMondGForceCsvDataProvider(lap, "lap1");
                    var reader = new LeMondDataReader(provider);
                    if (firstFile)
                    {
                        writer.StartActivity(reader.StartTime, TcxSport.Biking);
                        firstFile = false;
                    }

                    writer.StartLap(reader.StartTime);

                    bool firstPoint = true;
                    foreach (var point in reader.DataPoints)
                    {
                        if (firstPoint)
                        {
                            // adding a fake first point becuase strava seems
                            // to like seeing seconds 0:00-1:00 for a minute instead of 0:01-1:00
                            // this new point will actually give us 61 points, but will be considerd
                            // a full minute
                            WriteTrackPoint(writer, stats, reader.StartTime - oneSecond, point);
                            firstPoint = false;
                        }
                        WriteTrackPoint(writer, stats, reader.StartTime, point);
                    }

                    stats = writer.EndLap();
                }
                writer.EndActivity();
                writer.EndTcx();
            }
        }

        private static void WriteTrackPoint(TcxWriter writer, LapStats stats, DateTime baseTime, LeMondDataPoint point)
        {
            writer.StartTrackPoint();
            writer.WriteTrackPointTime(baseTime + point.ElapsedTime);
            writer.WriteTrackPointCadence(point.CadenceRotationsPerMinute);
            writer.WriteTrackPointElapsedCalories(point.ElapsedCalories + stats.Calories);
            writer.WriteTrackPointElapsedDistanceMeters(point.DistanceKilometers * MetersPerKilometer + stats.DistanceMeters);
            writer.WriteTrackPointHeartRateBpm(point.HeartRateBeatsPerMinute);
            writer.WriteTrackPointPowerWatts(point.PowerWatts);
            writer.WriteTrackPointSpeedMetersPerSecond(point.SpeedKilometersPerHour * MetersPerKilometer * HoursPerSecond);
            writer.EndTrackPoint();
        }

    }
}
