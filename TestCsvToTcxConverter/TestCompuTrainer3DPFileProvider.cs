﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using LeMondCsvToTcxConverter;
using System.IO;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestCompuTrainer3DPFileProvider
    {
        [TestMethod]
        public void TestProviderHeaderRead()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TestCsvToTcxConverter.John-CompuTrainer_TimeTrial-2012-01-04-21-44-16.3dp");
            var provider = new CompuTrainer3DPFileProvider(stream, "resourceStream");
            Assert.AreEqual(new DateTime(2012, 01, 04, 21, 27, 0, DateTimeKind.Local), provider.StartTime);
            Assert.AreEqual("John", provider.UserName);
            Assert.AreEqual(43, provider.Age);
            Assert.AreEqual(75.30, Math.Round(provider.WeightKilos, 2));
            Assert.AreEqual(200, provider.UpperHeartRate);
            Assert.AreEqual(0, provider.LowerHeartRate);

            bool foundPoint1 = false;
            bool foundPoint2 = false;
            foreach (var point in provider.Samples)
            {
                if (point.ElapsedTimeMilisecond == 0)
                {
                    Assert.AreEqual(78, point.Cadence);
                    Assert.AreEqual(0, point.DistanceKilometer);
                    Assert.AreEqual(0, point.GradePercent);
                    Assert.AreEqual(121, point.HeartRate);
                    Assert.AreEqual(252, point.PowerWatts);
                    Assert.AreEqual(21.0, point.SpeedMph);
                    foundPoint1 = true;
                }
                else if (point.ElapsedTimeMilisecond == 2347)
                {
                    Assert.AreEqual(85, point.Cadence);
                    Assert.AreEqual(0.0229, Math.Round(point.DistanceKilometer, 4));
                    Assert.AreEqual(-0.0037, Math.Round(point.GradePercent, 4));
                    Assert.AreEqual(124, point.HeartRate);
                    Assert.AreEqual(359, point.PowerWatts);
                    Assert.AreEqual(22.48, Math.Round(point.SpeedMph, 2));
                    foundPoint1 = true;
                }
            

            }
        }

    }
}