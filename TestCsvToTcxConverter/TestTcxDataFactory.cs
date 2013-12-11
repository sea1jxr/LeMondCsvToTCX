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
        SourcedStream computrainer3DPReader = new SourcedStream() { Source = "joe.3dp", Stream = Util.CreateStream(string.Empty) };
        SourcedStream computrainerTXTReader = new SourcedStream() { Source = "joe.CdF.txt", Stream = Util.CreateStream(string.Empty) };
        TestTcxData lemondData = new TestTcxData();
        TestTcxData computrainer3DPData = new TestTcxData();
        TestTcxData computrainerTXTData = new TestTcxData();

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
                    computrainer3DPData.Reader = r;
                    return computrainer3DPData;
                },
                (r) =>
                {
                    computrainerTXTData.Reader = r;
                    return computrainerTXTData;
                });
        }

        [TestMethod]
        public void ErrorOnBadExtension()
        {
           var factory = new TcxDataFactory(null, null, null);
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
        public void CreatesCompuTrainer3DPTcxData()
        {
            var actualData = testFactory.Create(computrainer3DPReader);
            Assert.AreSame(computrainer3DPData, actualData);
            Assert.AreSame(computrainer3DPReader, ((TestTcxData)actualData).Reader);
        }

        [TestMethod]
        public void CreatesCompuTrainerTXTTcxData()
        {
            var actualData = testFactory.Create(computrainerTXTReader);
            Assert.AreSame(computrainerTXTData, actualData);
            Assert.AreSame(computrainerTXTReader, ((TestTcxData)actualData).Reader);
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
