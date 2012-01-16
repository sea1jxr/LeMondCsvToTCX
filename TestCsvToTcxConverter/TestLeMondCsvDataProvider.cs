using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeMondCsvToTcxConverter;
using System.IO;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestLeMondCsvDataProvider
    {
        SourcedReader emptyFile = new SourcedReader()
        {
            TextReader = new StringReader(string.Empty),
            Source = "emptyFile"
        };

        SourcedReader nonLeMondFile = new SourcedReader()
        {
            TextReader = new StringReader("Armstrong,FW 1.00,HW 1.00,gforce,120102,16:31"),
            Source = "nonLeMondFile"
        };

        SourcedReader nonRecognizedType = new SourcedReader()
        {
            TextReader = new StringReader("LeMond"),
            Source = "nonRecognizedType"
        };

        SourcedReader gforceType = new SourcedReader()
        {
            TextReader = new StringReader("LeMond,,,gforce,120102,16:31\r\nTIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR"),
            Source = "gforceType"
        };

        SourcedReader revolutionType = new SourcedReader()
        {
            TextReader = new StringReader("LeMond,Revolution,,,30-Mar,18:33:17\r\nTIME,SPEED,DIST,POWER,HEART RATE,CADENCE,CALORIES,TARGET"),
            Source = "revolutionType"
        };

        [TestMethod]
        public void ErrorOnEmptyFile()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondCsvDataProvider.Create(emptyFile));
            StringAssert.Contains(e.Message, "it is empty");
        }

        [TestMethod]
        public void ErrorOnNonLeMondFile()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondCsvDataProvider.Create(nonLeMondFile));
            StringAssert.Contains(e.Message, "'LeMond'");
        }

        [TestMethod]
        public void ErrorOnNonRecognizedTypeOfLeMondFile()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => LeMondCsvDataProvider.Create(nonRecognizedType));
            StringAssert.Contains(e.Message, "LeMond device");
        }

        [TestMethod]
        public void TestCreatesGForceType()
        {
            var provider = LeMondCsvDataProvider.Create(gforceType);
            Assert.AreEqual(typeof(LeMondGForceCsvDataProvider), provider.GetType());
        }

        [TestMethod]
        public void TestCreatesRevolutionType()
        {
            var provider = LeMondCsvDataProvider.Create(revolutionType);
            Assert.AreEqual(typeof(LeMondRevolutionCsvDataProvider), provider.GetType());
        }

    }
}
