using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace LeMondCsvToTcxConverter
{
    public enum TcxSport
    {
        Biking,
        Running,
        Other,
    }

    public class LapStats
    {
        public double TotalTimeSeconds { get; set; }
        public double DistanceMeters { get; set; }
        public int Calories { get; set; }
        //public int MaxCadence { get; set; }
        //public double AverageSpeedMetersPerSecond { get; set; }
        //public int AveragePowerWatts { get; set; }
        //public int MaxPowerWatts { get; set; }
    }

    public class TcxWriter : IDisposable
    {
        private const string TcxV2XmlNamespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
        private const string ActivityExtensionsV2XmlNamespace = "http://www.garmin.com/xmlschemas/ActivityExtension/v2";

        private bool tcxStarted;
        private XmlWriter xmlWriter;
        private bool inActivity;
        private List<LapPoint> lapPoints;

        public TcxWriter(TextWriter textWriter)
        {
            xmlWriter = XmlWriter.Create(textWriter);
        }

        public void StartTcx()
        {
            if(tcxStarted)
            {
                throw new InvalidOperationException("Can't start the TCX twice");
            }
            tcxStarted = true;

            xmlWriter.WriteStartElement("TrainingCenterDatabase", TcxV2XmlNamespace);
            xmlWriter.WriteStartElement("Activities", TcxV2XmlNamespace);

        }

        public void EndTcx()
        {
            if(!tcxStarted)
            {
                throw new InvalidOperationException("Can't end a TCX that wasn't started");
            }

            // </Activities>
            xmlWriter.WriteEndElement();
            // </TrainingCenterDatabase>
            xmlWriter.WriteEndElement();
        }

        public void StartActivity(DateTime startTime, TcxSport sport)
        {
            if(!tcxStarted)
            {
                StartTcx();
            }

            if(inActivity)
            {
                EndActivity();
            }
            inActivity = true;

            xmlWriter.WriteStartElement("Activity", TcxV2XmlNamespace);
            xmlWriter.WriteAttributeString("Sport", sport.ToString());

            // write the Id
            xmlWriter.WriteStartElement("Id", TcxV2XmlNamespace);
            xmlWriter.WriteValue(startTime);
            xmlWriter.WriteEndElement();
        }

        public void EndActivity()
        {
            inActivity = false;
            
            // </Activity>
            xmlWriter.WriteEndElement();
        }

        public void StartLap(DateTime startTime)
        {
            if (this.lapPoints != null)
            {
                throw new InvalidOperationException("Can't start a lap before calling EndLap on the previous lap");
            }

            xmlWriter.WriteStartElement("Lap", TcxV2XmlNamespace);
            
            // StartTime attribute
            xmlWriter.WriteStartAttribute("StartTime");
            xmlWriter.WriteValue(startTime.ToUniversalTime());
            xmlWriter.WriteEndAttribute();

            lapPoints = new List<LapPoint>();
        }

        public LapStats EndLap()
        {
            LapStats stats = new LapStats();

            // write out the stats that must be writen out first
            // TotalTime
            stats.TotalTimeSeconds = (lapPoints.Last().Time.Value - lapPoints.First().Time.Value).TotalSeconds;
            xmlWriter.WriteStartElement("TotalTimeSeconds", TcxV2XmlNamespace);
            xmlWriter.WriteValue(stats.TotalTimeSeconds);
            xmlWriter.WriteEndElement();

            // DistanceMeters
            stats.DistanceMeters = lapPoints.Last().ElapsedDistanceMeters.Value - lapPoints.First().ElapsedDistanceMeters.Value;
            xmlWriter.WriteStartElement("DistanceMeters", TcxV2XmlNamespace);
            xmlWriter.WriteValue(stats.DistanceMeters);
            xmlWriter.WriteEndElement();

            // Calories
            stats.Calories = lapPoints.Last().ElapsedCalories.Value;
            xmlWriter.WriteStartElement("Calories", TcxV2XmlNamespace);
            xmlWriter.WriteValue(stats.Calories);
            xmlWriter.WriteEndElement();

            // Intensity
            xmlWriter.WriteStartElement("Intensity", TcxV2XmlNamespace);
            xmlWriter.WriteValue("Active");
            xmlWriter.WriteEndElement();

            //TriggerMethod
            xmlWriter.WriteStartElement("TriggerMethod", TcxV2XmlNamespace);
            xmlWriter.WriteValue("Manual");
            xmlWriter.WriteEndElement();

            // write out each of the track points
            xmlWriter.WriteStartElement("Track", TcxV2XmlNamespace);
            foreach (var point in lapPoints)
            {
                WriteTrackPoint(point);
            }
            xmlWriter.WriteEndElement();


            //// write out the lap extension stats
            //xmlWriter.WriteStartElement("Extensions", TcxV2XmlNamespace);

            //stats.MaxCadence = lapPoints.Max(lp => lp.Cadence.Value);
            //xmlWriter.WriteStartElement("LX", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteStartElement("MaxBikeCadence", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteValue(stats.MaxCadence);
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteEndElement();

            //stats.AverageSpeedMetersPerSecond = lapPoints.Average(lp => lp.SpeedMetersPerSecond.Value);
            //xmlWriter.WriteStartElement("LX", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteStartElement("AvgSpeed", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteValue(stats.AverageSpeedMetersPerSecond);
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteEndElement();

            //stats.AveragePowerWatts = (int)Math.Round(lapPoints.Average(lp => lp.PowerWatts.Value));
            //xmlWriter.WriteStartElement("LX", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteStartElement("AvgWatts", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteValue(stats.AveragePowerWatts);
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteEndElement();

            //stats.MaxPowerWatts = lapPoints.Max(lp => lp.PowerWatts.Value);
            //xmlWriter.WriteStartElement("LX", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteStartElement("MaxWatts", ActivityExtensionsV2XmlNamespace);
            //xmlWriter.WriteValue(stats.MaxPowerWatts);
            //xmlWriter.WriteEndElement();
            //xmlWriter.WriteEndElement();

            //// </Extensions>
            //xmlWriter.WriteEndElement();

            // </Lap>
            xmlWriter.WriteEndElement();

            this.lapPoints = null;
            return stats;
        }

        private void WriteTrackPoint(LapPoint point)
        {
            xmlWriter.WriteStartElement("Trackpoint", TcxV2XmlNamespace);
            
            xmlWriter.WriteStartElement("Time", TcxV2XmlNamespace);
            xmlWriter.WriteValue(point.Time.Value.ToUniversalTime());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("DistanceMeters", TcxV2XmlNamespace);
            xmlWriter.WriteValue(point.ElapsedDistanceMeters.Value);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("HeartRateBpm", TcxV2XmlNamespace);
            WriteValueElement(point.HeartRateBpm.Value);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Cadence", TcxV2XmlNamespace);
            xmlWriter.WriteValue(point.Cadence.Value);
            xmlWriter.WriteEndElement();

            // Extensions
            xmlWriter.WriteStartElement("Extensions", TcxV2XmlNamespace);
            xmlWriter.WriteStartElement("TPX", ActivityExtensionsV2XmlNamespace);
            
            xmlWriter.WriteStartElement("Speed", ActivityExtensionsV2XmlNamespace);
            xmlWriter.WriteValue(point.SpeedMetersPerSecond.Value);
            xmlWriter.WriteEndElement();
            
            xmlWriter.WriteStartElement("Watts", ActivityExtensionsV2XmlNamespace);
            xmlWriter.WriteValue(point.PowerWatts.Value);
            xmlWriter.WriteEndElement();
            
            // </TPX>
            xmlWriter.WriteEndElement();
            // </Extensions>
            xmlWriter.WriteEndElement();
            
            // </TrackPoint>
            xmlWriter.WriteEndElement();
        }

        private void WriteValueElement(object value)
        {
            xmlWriter.WriteStartElement("Value", TcxV2XmlNamespace);
            xmlWriter.WriteValue(value);
            xmlWriter.WriteEndElement();
        }

        public void StartTrackPoint()
        {
            lapPoints.Add(new LapPoint());
        }

        public void WriteTrackPointTime(DateTime time)
        {
            lapPoints.Last().Time = time;
        }

        public void WriteTrackPointElapsedDistanceMeters(double elapsedDistanceMeters)
        {
            lapPoints.Last().ElapsedDistanceMeters = elapsedDistanceMeters;
        }

        public void WriteTrackPointHeartRateBpm(int heartRateBpm)
        {
            lapPoints.Last().HeartRateBpm = heartRateBpm;
        }

        public void WriteTrackPointCadence(int cadence)
        {
            lapPoints.Last().Cadence = cadence;
        }

        public void WriteTrackPointSpeedMetersPerSecond(double speedMetersPerSecond)
        {
            lapPoints.Last().SpeedMetersPerSecond = speedMetersPerSecond;
        }

        public void WriteTrackPointPowerWatts(int powerWatts)
        {
            lapPoints.Last().PowerWatts = powerWatts;
        }

        public void WriteTrackPointElapsedCalories(int elapsedCalories)
        {
            lapPoints.Last().ElapsedCalories = elapsedCalories;
        }

        public void EndTrackPoint()
        {
            // NoOp for now
        }



        public void Dispose()
        {
            if (xmlWriter != null)
            {
                ((IDisposable)xmlWriter).Dispose();
                xmlWriter = null;
            }
        }


        private class LapPoint
        {
            public DateTime? Time { get; set; }
            public double? ElapsedDistanceMeters { get; set; }
            public int? HeartRateBpm { get; set; }
            public int? Cadence { get; set; }
            public double? SpeedMetersPerSecond { get; set; }
            public int? PowerWatts { get; set; }
            public int? ElapsedCalories { get; set; }
        }
    }


}
