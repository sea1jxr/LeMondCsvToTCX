using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeMondCsvToTcxConverter;
using System;
using System.Linq;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestLeMondGForceCsvDataProvider
    {
        TextReader emptyFile;
        TextReader nonLeMondFile;
        TextReader wrongColumnHeadings;
        TextReader goodOneDataPoint;
        [TestInitialize]
        public void TestInitialize()
        {
            emptyFile = new StringReader(string.Empty);
            nonLeMondFile = new StringReader("Armstrong,FW 1.00,HW 1.00,gforce,120102,16:31");
            wrongColumnHeadings = new StringReader(
@"LeMond,FW 1.00,HW 1.00,gforce,120102,16:31
TIMEz,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
");

            goodOneDataPoint = new StringReader(
@"LeMond,FW 1.00,HW 1.00,gforce,120102,16:31
TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
00:00:01,2.0,3.0,4,5,6,7,8,9
");
        }

        [TestMethod]
        public void ErrorOnEmptyFile()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondGForceCsvDataProvider(emptyFile, "emptyFile"));
            StringAssert.Contains(e.Message, "it is empty");
        }

        [TestMethod]
        public void ErrorOnNonLeMondFile()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondGForceCsvDataProvider(nonLeMondFile, "nonLeMondFile"));
            StringAssert.Contains(e.Message, "'LeMond'");
        }

        [TestMethod]
        public void ErrorOnWrongDataColumnHeadingsFile()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondGForceCsvDataProvider(wrongColumnHeadings, "wrongColumnHeadings"));
            StringAssert.Contains(e.Message, "correct data fields");
        }

        [TestMethod]
        public void TestDateAndTimeAndLines()
        {
            var provider = new LeMondGForceCsvDataProvider(goodOneDataPoint, "goodOneDataPoint");
            Assert.AreEqual("16:31", provider.StartTime);
            Assert.AreEqual("120102", provider.StartDate);

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


    }
}
