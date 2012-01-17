using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LeMondCsvToTcxConverter;
using Microsoft.VisualBasic.FileIO;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestLeMondRevolutionCsvDataProvider
    {
        SourcedReader wrongColumnHeadings = new SourcedReader();
        SourcedReader goodOneDataPoint = new SourcedReader();
        [TestInitialize]
        public void TestInitialize()
        {
            wrongColumnHeadings.TextReader = new StringReader(
@"LeMond, Revolution,FW 50,HW 02,30-Mar,18:33:17,Alt 30,Temp 25,Hum 45,Tire 2105,CF 150
TIMEz,SPEED,DIST,POWER,HEART RATE,CADENCE,CALORIES,TARGET,,,
");
            wrongColumnHeadings.Source = "wrongColumnHeadings";

            goodOneDataPoint.TextReader = new StringReader(
@"LeMond, Revolution,FW 50,HW 02,30-Mar,18:33:17,Alt 30,Temp 25,Hum 45,Tire 2105,CF 150
TIME,SPEED,DIST,POWER,HEART RATE,CADENCE,CALORIES,TARGET,,,
0:00:01,2.0,3.0,4,5,6,7,0,,,
");
            goodOneDataPoint.Source = "goodOneDataPoint";
        }

        [TestMethod]
        public void ErrorOnWrongDataColumnHeadingsFile()
        {
            var h = new LeMondConcreateProviderCtorHelper(wrongColumnHeadings);
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondRevolutionCsvDataProvider(h.SourceName, h.Parser, h.FirstRow));
            StringAssert.Contains(e.Message, "correct data fields");
        }

        [TestMethod]
        public void TestDateAndTimeAndLines()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondRevolutionCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            int year = CalculateYear(3);
            Assert.AreEqual(new DateTime(year, 3, 30, 18, 33, 17, DateTimeKind.Local), provider.StartTime);

            // lines
            // Single() will make sure we have one and only one line
            var line = provider.DataLines.Single();
            Assert.AreEqual(line.Time, "0:00:01");
            Assert.AreEqual(line.Speed, "2.0");
            Assert.AreEqual(line.Distance, "3.0");
            Assert.AreEqual(line.Power, "4");
            Assert.AreEqual(line.HeartRate, "5");
            Assert.AreEqual(line.Rpm, "6");
            Assert.AreEqual(line.Calories, "7");
        }

        private static int CalculateYear(int month)
        {
            return DateTime.Now.Month < month ? DateTime.Now.Year - 1 : DateTime.Now.Year;
        }

        int year, month, day, hour, minute, second;

        [TestMethod]
        public void TestAbbreviatedMonthFormat()
        {
            LeMondRevolutionCsvDataProvider.ParseDate("31-Dec", out year, out month, out day);
            Assert.AreEqual(31, day);
            Assert.AreEqual(12, month);
            Assert.AreEqual(CalculateYear(month), year);

        }

        [TestMethod]
        public void TestShortNoYearFormat()
        {
            LeMondRevolutionCsvDataProvider.ParseDate("12/31", out year, out month, out day);
            Assert.AreEqual(31, day);
            Assert.AreEqual(12, month);
            Assert.AreEqual(CalculateYear(month), year);

        }

        [TestMethod]
        public void ErrorOnDateToShort()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseDate("12", out year, out month, out day));
            StringAssert.Contains(e.Message, "DD-MMM");
        }

        [TestMethod]
        public void ErrorOnDateToLong()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseDate("1234567", out year, out month, out day));
            StringAssert.Contains(e.Message, "DD-MMM");
        }

        [TestMethod]
        public void ErrorOnDateEmpty()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseDate("", out year, out month, out day));
            StringAssert.Contains(e.Message, "DD-MMM");
        }

        [TestMethod]
        public void ErrorOnDateNull()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseDate(null, out year, out month, out day));
            StringAssert.Contains(e.Message, "DD-MMM");
        }

        [TestMethod]
        public void ErrorOnDayNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseDate("ab-Dec", out year, out month, out day));
            StringAssert.Contains(e.Message, "DD-MMM");
        }

        [TestMethod]
        public void ErrorOnMonthNotCorrectAbbreviation()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseDate("12-Tov", out year, out month, out day));
            StringAssert.Contains(e.Message, "DD-MMM");
        }

        [TestMethod]
        public void ErrorOnTimeToShort()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime("12", out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }

        [TestMethod]
        public void ErrorOnTimeToLong()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime("12::34::33", out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }

        [TestMethod]
        public void ErrorOnTimeEmpty()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime("", out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }

        [TestMethod]
        public void ErrorOnTimeNull()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime(null, out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }

        [TestMethod]
        public void ErrorOnHourNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime("ab:30", out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }

        [TestMethod]
        public void ErrorOnMinuteNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime("10:cd", out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }

        [TestMethod]
        public void ErrorOnSecondNotNumerical()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondRevolutionCsvDataProvider.ParseTime("10:30:ef", out hour, out minute, out second));
            StringAssert.Contains(e.Message, "HH:MM:SS");
        }
    }
}
