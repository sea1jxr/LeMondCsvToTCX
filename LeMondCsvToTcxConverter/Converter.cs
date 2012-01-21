using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LeMondCsvToTcxConverter
{
    public class Converter
    {
        
        public void WriteTcxFile(IEnumerable<SourcedReader> laps, TextWriter textWriter)
        {
            using (TcxWriter writer = new TcxWriter(textWriter))
            {
                writer.StartTcx();
                bool firstFile = true;
                LapStats stats = new LapStats() { Calories = 0, DistanceMeters = 0, TotalTimeSeconds = 0 };
                foreach (var lap in laps)
                {
                    var provider = LeMondCsvDataProvider.Create(lap);
                    var reader = new LeMondDataReader(provider);
                    var data = new LeMondTcxData(reader);
                    if (firstFile)
                    {
                        writer.StartActivity(data.StartTime, data.Sport);
                        firstFile = false;
                    }

                    writer.StartLap(data.StartTime);

                    foreach (var point in data.TrackPoints)
                    {
                        WriteTrackPoint(writer, stats, point);
                    }

                    stats = writer.EndLap();
                }
                writer.EndActivity();
                writer.EndTcx();
            }
        }

        private static void WriteTrackPoint(TcxWriter writer, LapStats stats, TcxTrackPoint point)
        {
            writer.StartTrackPoint();
            writer.WriteTrackPointTime(point.Time);
            writer.WriteTrackPointCadence(point.CadenceRpm);
            writer.WriteTrackPointElapsedCalories(point.ElapsedCalories + stats.Calories);
            writer.WriteTrackPointElapsedDistanceMeters(point.ElapsedDistanceMeters + stats.DistanceMeters);
            writer.WriteTrackPointHeartRateBpm(point.HeartRateBpm);
            writer.WriteTrackPointPowerWatts(point.PowerWatts);
            writer.WriteTrackPointSpeedMetersPerSecond(point.SpeedMetersPerSecond);
            writer.EndTrackPoint();
        }

    }
}
