using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LeMondCsvToTcxConverter;
using System.Xml.Linq;
using System.Diagnostics;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestTcxWriter
    {
        StringBuilder result = new StringBuilder();
        TextWriter textWriter;
        static readonly DateTime point1Time = new DateTime(7, 8, 9, 10, 11, 12, DateTimeKind.Utc);
        static readonly DateTime point2Time = new DateTime(19, 12, 21, 22, 23, 24, DateTimeKind.Utc);
        static readonly XNamespace trainingCenterv2 = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
        static readonly XNamespace trainingExtensionsv2 = "http://www.garmin.com/xmlschemas/ActivityExtension/v2";

        [TestInitialize]
        public void TestInitialize()
        {
            textWriter = new StringWriter(result);
        }

        [TestMethod]
        public void TestEmptyTcx()
        {
            var writer = new TcxWriter(textWriter);
            writer.Dispose();
            Assert.AreEqual(string.Empty, result.ToString());
        }

        [TestMethod]
        public void TestEmptyActivityTcx()
        {
            var expectedActivityTime = new DateTime(1, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            var writer = new TcxWriter(textWriter);
            writer.StartActivity(expectedActivityTime, TcxSport.Other);
            writer.EndActivity();
            writer.Dispose();
            
            var root = XElement.Parse(result.ToString());
            var activities = GetActivities(root);
            var activity = activities.Single();
            Assert.AreEqual(activity.Sport, "Other");
            Assert.AreEqual("0001-02-03T04:05:06Z", activity.Id);
            Assert.AreEqual(0, activity.Laps.Count());

        }

        [TestMethod]
        public void TestSingleLapSingleNoTrackTcx()
        {
            var expectedActivityTime = new DateTime(6, 5, 4, 3, 2, 1, DateTimeKind.Utc);
            var writer = new TcxWriter(textWriter);
            writer.StartActivity(expectedActivityTime, TcxSport.Biking);
            writer.StartLap(expectedActivityTime);
            writer.EndLap();
            writer.EndActivity();
            writer.Dispose();

            var root = XElement.Parse(result.ToString());
            var activities = GetActivities(root);
            var activity = activities.Single();
            Assert.AreEqual(activity.Sport, "Biking");
            Assert.AreEqual("0006-05-04T03:02:01Z", activity.Id);

            var lap = activity.Laps.Single();
            Assert.AreEqual("0", lap.TotalTimeSeconds);
            Assert.AreEqual("0", lap.DistanceMeters);
            Assert.AreEqual("0", lap.Calories);
            Assert.AreEqual("Active", lap.Intensity);
            Assert.AreEqual("Manual", lap.TriggerMethod);
        }

        [TestMethod]
        public void TestTwoLapsWithTwoTrackPointsEachTcx()
        {
            var writer = new TcxWriter(textWriter);
            Action writeTrackPoints = () =>
            {
                writer.StartTrackPoint();
                writer.WriteTrackPointCadence(1);
                writer.WriteTrackPointElapsedCalories(2);
                writer.WriteTrackPointElapsedDistanceMeters(3.3);
                writer.WriteTrackPointHeartRateBpm(4);
                writer.WriteTrackPointPowerWatts(5);
                writer.WriteTrackPointSpeedMetersPerSecond(6.6);
                writer.WriteTrackPointTime(point1Time);
                writer.EndTrackPoint();

                writer.StartTrackPoint();
                writer.WriteTrackPointCadence(13);
                writer.WriteTrackPointElapsedCalories(14);
                writer.WriteTrackPointElapsedDistanceMeters(15.15);
                writer.WriteTrackPointHeartRateBpm(16);
                writer.WriteTrackPointPowerWatts(17);
                writer.WriteTrackPointSpeedMetersPerSecond(18.18);
                writer.WriteTrackPointTime(point2Time);
                writer.EndTrackPoint();
            };

            var expectedActivityTime = new DateTime(6, 5, 4, 3, 2, 1, DateTimeKind.Utc);
            writer.StartActivity(expectedActivityTime, TcxSport.Biking);
            
            var expectedLap1Time = new DateTime(1, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            writer.StartLap(expectedLap1Time);
            writeTrackPoints();
            writer.EndLap();

            var expectedLap2Time = new DateTime(1, 2, 3, 6, 5, 4, DateTimeKind.Utc);
            writer.StartLap(expectedLap2Time);
            writeTrackPoints();
            writer.EndLap();

            writer.EndActivity();
            writer.Dispose();

            var root = XElement.Parse(result.ToString());
            var activities = GetActivities(root);
            var activity = activities.Single();
            Assert.AreEqual(activity.Sport, "Biking");
            Assert.AreEqual("0006-05-04T03:02:01Z", activity.Id);

            Assert.AreEqual(2, activity.Laps.Count());
            AssertLap(activity.Laps.First());
            AssertLap(activity.Laps.Last());

        }

        private void AssertLap(TcxLap lap)
        {
            Assert.AreEqual((point2Time - point1Time).TotalSeconds.ToString(), lap.TotalTimeSeconds);
            Assert.AreEqual("15.15", lap.DistanceMeters);
            Assert.AreEqual("14", lap.Calories);
            Assert.AreEqual("Active", lap.Intensity);
            Assert.AreEqual("Manual", lap.TriggerMethod);

            Assert.AreEqual(2, lap.TrackPoints.Count());
            
            // track point 1
            var point = lap.TrackPoints.First();
            Assert.AreEqual("1", point.Cadence);
            Assert.AreEqual("3.3", point.DistanceMeters);
            Assert.AreEqual("4", point.HeartRateBpm);
            Assert.AreEqual("1", point.Watts);
            Assert.AreEqual("1", point.Speed);
            Assert.AreEqual("", point.Time);

            // track point 2
            point = lap.TrackPoints.Last();

        }
        private IEnumerable<TcxActivity> GetActivities(XElement root)
        {
            var activities = root.DescendantsAndSelf().Where(e => e.Name == trainingCenterv2 + "Activities");
            return activities.Single().Elements().Select(e => new TcxActivity(e));
        }

        private class TcxActivity
        {
            XElement activityElement;
            public TcxActivity(XElement activityElement)
            {
                Debug.Assert(activityElement.Name == trainingCenterv2 + "Activity");
                this.activityElement = activityElement;
            }

            public string Sport
            {
                get
                {
                    return activityElement.Attribute("Sport").Value;
                }
            }

            public string Id
            {
                get
                {
                    return activityElement.Element(trainingCenterv2 + "Id").Value;
                }
            }

            public IEnumerable<TcxLap> Laps
            {
                get
                {
                    return activityElement.Descendants().Where(e => e.Name == trainingCenterv2 + "Lap").Select(e => new TcxLap(e));
                }
            }
        }

        private class TcxLap
        {
            private XElement lapElement;
            public TcxLap(XElement lapElement)
            {
                Debug.Assert(lapElement.Name == trainingCenterv2 + "Lap");
                this.lapElement = lapElement;
            }
            
            public string TotalTimeSeconds
            {
                get
                {
                    return lapElement.Element(trainingCenterv2 + "TotalTimeSeconds").Value;
                }
            }

            public string DistanceMeters
            {
                get
                {
                    return lapElement.Element(trainingCenterv2 + "DistanceMeters").Value;
                }
            }

            public string Calories
            {
                get
                {
                    return lapElement.Element(trainingCenterv2 + "Calories").Value;
                }
            }

            public string Intensity
            {
                get
                {
                    return lapElement.Element(trainingCenterv2 + "Intensity").Value;
                }
            }

            public string TriggerMethod
            {
                get
                {
                    return lapElement.Element(trainingCenterv2 + "TriggerMethod").Value;
                }
            }

            public IEnumerable<TcxTrackPoint> TrackPoints
            {
                get
                {
                    var track = lapElement.Element(trainingCenterv2 + "Track");
                    return track.Elements().Where(e => e.Name == trainingCenterv2 + "Trackpoint").Select(tp => new TcxTrackPoint(tp));
                }
            }
        }

        private class TcxTrackPoint
        {
            private XElement trackPointElement;
            public TcxTrackPoint(XElement trackPointElement)
            {
                this.trackPointElement = trackPointElement;
            }

            public string Time
            {
                get
                {
                    return trackPointElement.Element(trainingCenterv2 + "Time").Value;
                }
            }

            public string DistanceMeters
            {
                get
                {
                    return trackPointElement.Element(trainingCenterv2 + "DistanceMeters").Value;
                }
            }

            public string HeartRateBpm
            {
                get
                {
                    var heartRate = trackPointElement.Element(trainingCenterv2 + "HeartRateBpm");
                    return heartRate.Element(trainingExtensionsv2 + "Value").Value;
                }
            }

            public string Cadence
            {
                get
                {
                    return trackPointElement.Element(trainingCenterv2 + "Cadence").Value;
                }
            }

            public string Speed
            {
                get
                {
                    return GetExtensionElement("Speed").Value;
                }
            }

            public string Watts
            {
                get
                {
                    return GetExtensionElement("Watts").Value;
                }
            }

            private XElement GetExtensionElement(string localName)
            {
                return trackPointElement.Element(trainingCenterv2 + "Extensions")
                        .Element(trainingExtensionsv2 + "TPX")
                        .Element(trainingExtensionsv2 + localName);
            }
        }
    }
}
