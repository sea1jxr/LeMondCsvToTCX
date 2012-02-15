using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertToTcx;
using System;
using System.Linq;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestLeMondGForceSTNCsvDataProvider
    {
        const string Version0_31 = "FW 0.31";
        SourcedStream wrongColumnHeadings = new SourcedStream();
        SourcedStream goodOneDataPoint = new SourcedStream();
        [TestInitialize]
        public void TestInitialize()
        {
            wrongColumnHeadings.Stream = Util.CreateStream(
@"LeMond,FW 0.25,HW 1.0,STN,111230,15:02,,,
TIMEz,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
");
            wrongColumnHeadings.Source = "wrongColumnHeadings";

            goodOneDataPoint.Stream = Util.CreateStream(
@"LeMond,FW 0.25,HW 1.0,STN,111230,15:02,,,
TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
00:00:01,2.0,3.0,4,5,6,7,8,9
");
            goodOneDataPoint.Source = "goodOneDataPoint";
        }



        [TestMethod]
        public void ErrorOnWrongDataColumnHeadingsFile()
        {
            var h = new LeMondConcreateProviderCtorHelper(wrongColumnHeadings);
            Exception e = ExceptionAssert.Throws<Exception>(() => new LeMondGForceSTNCsvDataProvider(h.SourceName, h.Parser, h.FirstRow));
            StringAssert.Contains(e.Message, "correct data fields");
        }

        [TestMethod]
        public void TestDateAndTimeAndLines()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondGForceSTNCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            Assert.AreEqual(new DateTime(2011, 12, 30, 15, 02, 0, DateTimeKind.Local), provider.StartTime);

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

        [TestMethod]
        public void TestFW25ConvertSpeedToKilometersPerHour()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondGForceSTNCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            double d = provider.ConvertSpeedToKilometersPerHour(6.2);
            Assert.AreEqual(10.0, Math.Round(d, 1));
        }

        [TestMethod]
        public void TestFW31ConvertSpeedToKilometersPerHour()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            h.FirstRow[1] = Version0_31;
            var provider = new LeMondGForceSTNCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            double d = provider.ConvertSpeedToKilometersPerHour(6.2);
            Assert.AreEqual(6.2, Math.Round(d, 1));
        }

        [TestMethod]
        public void TestFW25ConvertDistanceToKilometers()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            var provider = new LeMondGForceSTNCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            double d = provider.ConvertDistanceToKilometers(3.1);
            Assert.AreEqual(5.0, Math.Round(d, 1));
        }

        [TestMethod]
        public void TestFW31ConvertDistanceToKilometers()
        {
            var h = new LeMondConcreateProviderCtorHelper(goodOneDataPoint);
            h.FirstRow[1] = Version0_31;
            var provider = new LeMondGForceSTNCsvDataProvider(h.SourceName, h.Parser, h.FirstRow);
            double d = provider.ConvertDistanceToKilometers(3.1);
            Assert.AreEqual(3.1, Math.Round(d, 1));
        }
    }
}
