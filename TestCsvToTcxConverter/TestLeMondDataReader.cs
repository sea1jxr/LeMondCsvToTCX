using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertToTcx;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestLeMondDataReader
    {
        List<LeMondCsvDataLine> lines = new List<LeMondCsvDataLine>();
        MockLeMondDataProvider provider;

        [TestInitialize]
        public void TestInitialize()
        {
            provider = new MockLeMondDataProvider() { StartTime = new DateTime(2012, 01, 02, 16, 31, 0, DateTimeKind.Local), DataLines = lines };
        }

        [TestMethod]
        public void TestCtor()
        {
            LeMondDataReader reader = new LeMondDataReader(provider);
            Assert.AreEqual(new DateTime(2012, 1, 2, 16, 31, 0), reader.StartTime, "start time was not read correctly");
        }

        [TestMethod]
        public void TestDataLineToDataPoint()
        {
            var line = new LeMondCsvDataLine()
            {
                Calories = "1",
                Distance = "2.2",
                HeartRate = "3",
                Power = "4",
                Rpm = "5",
                Speed = "6.6",
                Time = "00:00:07",
            };
            lines.Add(line);

            var reader = new LeMondDataReader(provider);
            var point = reader.DataPoints.Single();
            
            Assert.AreEqual(point.ElapsedCalories, 1);
            Assert.AreEqual(point.DistanceKilometers, 2.2);
            Assert.AreEqual(point.HeartRateBeatsPerMinute, 3);
            Assert.AreEqual(point.PowerWatts, 4);
            Assert.AreEqual(point.CadenceRotationsPerMinute, 5);
            Assert.AreEqual(point.SpeedKilometersPerHour, 6.6);
            Assert.AreEqual(point.ElapsedTime, new TimeSpan(0, 0, 7));
        }
    }
}
