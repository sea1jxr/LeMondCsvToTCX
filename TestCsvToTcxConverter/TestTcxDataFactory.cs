using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertToTcx;
using System.IO;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestTcxDataFactory
    {
        SourcedStream lemondReader = new SourcedStream() { Source = "joe.csv", Stream = Util.CreateStream(string.Empty) };
        SourcedStream computrainerReader = new SourcedStream() { Source = "joe.3dp", Stream = Util.CreateStream(string.Empty) };
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
           Exception e = ExceptionAssert.Throws<Exception>(() => factory.Create(new SourcedStream() { Source = "joe.zzz" }));
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
            public SourcedStream Reader { get; set; }

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
