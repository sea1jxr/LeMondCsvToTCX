using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertToTcx;
using System;
using System.Linq;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestLeMondGForceCsvDataProvider
    {
        SourcedStream wrongColumnHeadings = new SourcedStream();
        SourcedStream goodOneDataPoint = new SourcedStream();
        [TestInitialize]
        public void TestInitialize()
        {
            wrongColumnHeadings.Stream = Util.CreateStream(
@"LeMond,FW 1.00,HW 1.00,gforce,120102,16:31
TIMEz,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
");
            wrongColumnHeadings.Source = "wrongColumnHeadings";

            goodOneDataPoint.Stream = Util.CreateStream(
@"LeMond,FW 1.00,HW 1.00,gforce,120102,16:31
TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
00:00:01,2.0,3.0,4,5,6,7,8,9
");
            goodOneDataPoint.Source = "goodOneDataPoint";
        }



        [TestMethod]
        public void ErrorOnWrongDataColumnHeadingsFile()
        {
            var h = new LeMondConcreateProviderCtorHelper(wrongColumnHeadings);
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondGForceCsvDataProvider(h.SourceName, h.Parser, h.FirstRow));
            StringAssert.Contains(e.Message, "correct data fields");
        }

        [TestMethod]
        public void TestDateAndTimeAndLines()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondGForceCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            Assert.AreEqual(new DateTime(2012, 01, 02, 16, 31, 0, DateTimeKind.Local), provider.StartTime);

            // lines
            // Single() will make sure we have one and only one line
            var line = provider.DataLines.Single();
            Assert.AreEqual(line.Time, "00:00:01");
            Assert.AreEqual(line.Speed, "2.0");
            Assert.AreEqual(line.Distance, "3.0");
            Assert.AreEqual(line.Power, "4");
            Assert.AreEqual(line.HeartRate, "5");
            Assert.AreEqual(line.Rpm, "6");
            Assert.AreEqual(line.Calories, "7");
        }

        int year, month, day, hour, minute;
        [TestMethod]
        public void ErrorOnDateToShort()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate("12", out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnDateToLong()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate("1234567", out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnDateEmpty()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate("", out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnDateNull()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate(null, out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnYearNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate("ab0102", out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnMonthNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate("12cd02", out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnDayNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseDate("1201ef", out year, out month, out day));
            StringAssert.Contains(e.Message, "YYMMDD");
        }

        [TestMethod]
        public void ErrorOnTimeToShort()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseTime("12", out hour, out minute));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void ErrorOnTimeToLong()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseTime("12::34", out hour, out minute));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void ErrorOnTimeEmpty()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseTime("", out hour, out minute));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void ErrorOnTimeNull()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseTime(null, out hour, out minute));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void ErrorOnHourNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseTime("ab:30", out hour, out minute));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void ErrorOnMinuteNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondGForceCsvDataProvider.ParseTime("10:cd", out hour, out minute));
            StringAssert.Contains(e.Message, "HH:MM");
        }

        [TestMethod]
        public void TestConvertSpeedToKilometersPerHour()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondGForceCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            double d = provider.ConvertSpeedToKilometersPerHour(10.0);
            Assert.AreEqual(10.0, d);
        }

        [TestMethod]
        public void TestConvertDistanceToKilometers()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondGForceCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            double d = provider.ConvertDistanceToKilometers(3.1);
            Assert.AreEqual(3.1, d);
        }
    }
}
