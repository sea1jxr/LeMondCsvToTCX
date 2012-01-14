using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeMondCsvToTcxConverter;

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
            provider = new MockLeMondDataProvider() { StartDate = "120102", StartTime = "16:31", DataLines = lines };
        }

        [TestMethod]
        public void TestCtor()
        {
            LeMondDataReader reader = new LeMondDataReader(provider);
            Assert.AreEqual(new DateTime(2012, 1, 2, 16, 31, 0), reader.StartTime, "start time was not read correctly");
        }

        [TestMethod]
        public void TestDateToShort()
        {
            provider.StartDate = "12";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestDateToLong()
        {
            provider.StartDate = "1234567";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestDateEmpty()
        {
            provider.StartDate = "";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestDateNull()
        {
            provider.StartDate = null;
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestYearNotNumerical()
        {
            provider.StartDate = "ab0102";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestMonthNotNumerical()
        {
            provider.StartDate = "12cd02";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestDayNotNumerical()
        {
            provider.StartDate = "1201ef";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void TestTimeToShort()
        {
            provider.StartTime = "12";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void TestTimeToLong()
        {
            provider.StartTime = "12::34";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void TestTimeEmpty()
        {
            provider.StartTime = "";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void TestTimeNull()
        {
            provider.StartTime = null;
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void TestHourNotNumerical()
        {
            provider.StartTime = "ab:30";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void TestMinuteNotNumerical()
        {
            provider.StartTime = "10:cd";
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondDataReader(provider));
            StringAssert.Contains(e.Message, "HH:MM");
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
