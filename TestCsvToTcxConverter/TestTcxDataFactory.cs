﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeMondCsvToTcxConverter;
using System.IO;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestTcxDataFactory
    {
        SourcedReader lemondReader = new SourcedReader() { Source = "joe.csv", TextReader = new StringReader(string.Empty) };
        SourcedReader computrainerReader = new SourcedReader() { Source = "joe.3dp", TextReader = new StringReader(string.Empty) };
        TestTcxData lemondData = new TestTcxData();
        TestTcxData computrainerData = new TestTcxData();

        TcxDataFactory testFactory;
        
        [TestInitialize]
        public void TestInitialize()
        {
            testFactory = new TcxDataFactory(
                (r) =>
                {
                    lemondData.Reader = r;
                    return lemondData;
                },
                (r) =>
                {
                    computrainerData.Reader = r;
                    return computrainerData;
                });
        }

        [TestMethod]
        public void ErrorOnBadExtension()
        {
           var factory = new TcxDataFactory(null, null);
           Exception e = ExceptionAssert.Throws<Exception>(() => factory.Create(new SourcedReader() { Source = "joe.zzz" }));
           StringAssert.Contains(e.Message, ".zzz");
           StringAssert.Contains(e.Message, "not a supported file type");
        }

        [TestMethod]
        public void CreatesLeMondTcxData()
        {
            var actualData = testFactory.Create(lemondReader);
            Assert.AreSame(lemondData, actualData);
            Assert.AreSame(lemondReader, ((TestTcxData)actualData).Reader);
        }

        [TestMethod]
        public void CreatesCompuTrainerTcxData()
        {
            var actualData = testFactory.Create(computrainerReader);
            Assert.AreSame(computrainerData, actualData);
            Assert.AreSame(computrainerReader, ((TestTcxData)actualData).Reader);
        }

        private class TestTcxData : ITcxData
        {
            public SourcedReader Reader { get; set; }

            public DateTime StartTime
            {
                get { throw new NotImplementedException(); }
            }

            public TcxSport Sport
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerable<TcxTrackPoint> TrackPoints
            {
                get { throw new NotImplementedException(); }
            }
        }

    }
}