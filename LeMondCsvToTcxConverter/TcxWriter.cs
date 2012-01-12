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
        private bool useUniversalTime;
        public TcxWriter(TextWriter textWriter, bool useUniversalTime)
        {
            this.xmlWriter = XmlWriter.Create(textWriter);
            this.useUniversalTime = useUniversalTime;
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
            xmlWriter.WriteValue(ConvertDateTime(startTime));
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
            xmlWriter.WriteValue(ConvertDateTime(startTime));
            xmlWriter.WriteEndAttribute();

            lapPoints = new List<LapPoint>();
        }

        public LapStats EndLap()
        {
            LapStats stats = new LapStats();

            // write out the stats that must be writen out first
            stats.TotalTimeSeconds = (lapPoints.Last().Time.Value - lapPoints.First().Time.Value).TotalSeconds;
            stats.DistanceMeters = lapPoints.Last().ElapsedDistanceMeters.Value - lapPoints.First().ElapsedDistanceMeters.Value;
            stats.Calories = lapPoints.Last().ElapsedCalories.Value;

            WriteElementAndValue("TotalTimeSeconds", TcxV2XmlNamespace, stats.TotalTimeSeconds);
            WriteElementAndValue("DistanceMeters", TcxV2XmlNamespace, stats.DistanceMeters);
            WriteElementAndValue("Calories", TcxV2XmlNamespace, stats.Calories);
            WriteElementAndValue("Intensity", TcxV2XmlNamespace, "Active");
            WriteElementAndValue("TriggerMethod", TcxV2XmlNamespace, "Manual");

            // write out each of the track points
            xmlWriter.WriteStartElement("Track", TcxV2XmlNamespace);
            foreach (var point in lapPoints)
            {
                WriteTrackPoint(point);
            }
            xmlWriter.WriteEndElement();

            // </Lap>
            xmlWriter.WriteEndElement();

            this.lapPoints = null;
            return stats;
        }

        private void WriteTrackPoint(LapPoint point)
        {
            xmlWriter.WriteStartElement("Trackpoint", TcxV2XmlNamespace);
            
            WriteElementAndValue("Time", TcxV2XmlNamespace, ConvertDateTime(point.Time.Value));
            WriteElementAndValue("DistanceMeters", TcxV2XmlNamespace, point.ElapsedDistanceMeters.Value);
            WriteElementAndValueElement("HeartRateBpm", TcxV2XmlNamespace, point.HeartRateBpm.Value);
            WriteElementAndValue("Cadence", TcxV2XmlNamespace, point.Cadence.Value);

            // Extensions
            xmlWriter.WriteStartElement("Extensions", TcxV2XmlNamespace);
            xmlWriter.WriteStartElement("TPX", ActivityExtensionsV2XmlNamespace);

            WriteElementAndValue("Speed", ActivityExtensionsV2XmlNamespace, point.SpeedMetersPerSecond.Value);
            WriteElementAndValue("Watts", ActivityExtensionsV2XmlNamespace, point.PowerWatts.Value);
            
            // </TPX>
            xmlWriter.WriteEndElement();
            // </Extensions>
            xmlWriter.WriteEndElement();
            
            // </TrackPoint>
            xmlWriter.WriteEndElement();
        }

        private object ConvertDateTime(DateTime dateTime)
        {
            if (useUniversalTime)
            {
                return dateTime.ToUniversalTime();
            }

            // makes GMT look like local time
            return new DateTime(dateTime.ToLocalTime().Ticks, DateTimeKind.Utc);
        }

        private void WriteElementAndValue(string localName, string xmlNamespace, object value)
        {
            xmlWriter.WriteStartElement(localName, xmlNamespace);
            xmlWriter.WriteValue(value);
            xmlWriter.WriteEndElement();
        }

        private void WriteElementAndValueElement(string localName, string xmlNamespace, object value)
        {
            xmlWriter.WriteStartElement(localName, xmlNamespace);
            WriteValueElement(value);
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
