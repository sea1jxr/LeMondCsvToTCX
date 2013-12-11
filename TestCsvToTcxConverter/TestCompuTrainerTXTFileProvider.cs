using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertToTcx;
using System.Linq;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestCompuTrainerTXTFileProvider
    {
        SourcedStream fixedWidth = new SourcedStream()
        {
            Source = "Tom-325Watts_150W2_27_03_ERG8.erg-2013-12-07-12-25-30.cdf.txt",
            Stream = Util.CreateStream(@"[USER DATA]
Mark Liversedge
AGE=43
WEIGHT=85.0 kg
LOWER HR=60
UPPER HR=185
drag factor=100
[END USER DATA]

number of records = 1669

ms speed watts rpm hr ss lss rss lpwr rpwr KM wind grade load lata rata pp cadence ss_raw

        47   5.79    22   1  60   0   0   0    0    0   0.0001    0   0.00  100    90    90      0  0  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000
        94   5.31   107   2  80   0   0   0    0    0   0.0002    0   0.00  100    90    90      0  0  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000  0.00000")
        };

        SourcedStream csvData = new SourcedStream()
        {
            Source = "Tom-325Watts_150W2_27_03_ERG8.erg-2013-12-07-12-25-30.cdf.txt",
            Stream = Util.CreateStream(@"[USER DATA]
Jaime
AGE=44
WEIGHT=178.0 pounds
LOWER HR=56
UPPER HR=177
drag factor=100
[END USER DATA]

[COURSE HEADER]
VERSION = 2
UNITS = ENGLISH
DESCRIPTION = 
[END COURSE HEADER]

[COURSE DATA]
     24.85        0.00        0.00
[END COURSE DATA]

number of records = 39614

ms speed watts rpm hr ss lss rss lpwr rpwr miles wind grade load lata rata pp cadence ss_raw 

""9156"",""17.38"",""269.6"",""92"",""60"",""64"",""64"",""64"",""50"",""50"",""0.0272"",""0"",""0.00"",""0"",""86"",""87"",""0"",""0"",""0.41996"",""0.63915"",""0.91252"",""1.14235"",""1.29645"",""1.40388"",""1.41831"",""1.27468"",""0.98686"",""0.66810"",""0.41015"",""0.29158"",""0.36420"",""0.61101"",""0.93300"",""1.18463"",""1.35064"",""1.44206"",""1.42231"",""1.24684"",""0.94783"",""0.64944"",""0.42888"",""0.38219""
""12219"",""18.54"",""208"",""96"",""70"",""64"",""64"",""64"",""48"",""52"",""0.0427"",""0"",""0.00"",""0"",""91"",""92"",""0"",""0"",""0.30391"",""0.37969"",""0.57604"",""0.78212"",""0.94488"",""1.05125"",""1.09429"",""1.02206"",""0.83133"",""0.60890"",""0.41562"",""0.29380"",""0.26565"",""0.38159"",""0.59750"",""0.82256"",""1.03130"",""1.15781"",""1.17000"",""1.06487"",""0.88371"",""0.67763"",""0.46975"",""0.34617""
")
    };
        [TestMethod]
        public void ReadSampleCount()
        {
            var proivder = new CompuTrainerTXTFileProvider(fixedWidth);
            Assert.AreEqual(1669, proivder.SampleCount, "sample count is wrong");
        }

        [TestMethod]
        public void ReadCsvSample()
        {
            var proivder = new CompuTrainerTXTFileProvider(csvData);
            var samples = proivder.Samples.ToList();
            Assert.AreEqual(2, samples.Count, "wrong number of samples");
            
            var sample = samples[0];
            Assert.AreEqual(60, sample.HeartRateBpm, "HeartRateBpm is wrong");
            Assert.AreEqual((uint)9156, sample.TimeMilisecondElapsed, "TimeMilisecondElapsed is wrong");
            Assert.AreEqual(270, sample.PowerWatts, "PowerWatts is wrong");
            Assert.AreEqual(92, sample.CadenceRpm, "CadenceRpm is wrong");
            Assert.AreEqual(Math.Round(ConvertDistance.MilesToKilometers(0.0272F), 4), Math.Round(sample.DistanceKilometerElapsed, 4), "DistanceKilometerElapsed is wrong");
            Assert.AreEqual(Math.Round(17.38 / ConvertDistance.KilometersPerMile, 2), Math.Round(sample.SpeedMph, 2), "SpeedMph is wrong");

            sample = samples[1];
            Assert.AreEqual(70, sample.HeartRateBpm, "HeartRateBpm is wrong");
            Assert.AreEqual((uint)12219, sample.TimeMilisecondElapsed, "TimeMilisecondElapsed is wrong");
            Assert.AreEqual(208, sample.PowerWatts, "PowerWatts is wrong");
            Assert.AreEqual(96, sample.CadenceRpm, "CadenceRpm is wrong");
            Assert.AreEqual(Math.Round(ConvertDistance.MilesToKilometers(0.0427F), 4), Math.Round(sample.DistanceKilometerElapsed, 4), "DistanceKilometerElapsed is wrong");
            Assert.AreEqual(Math.Round(18.54 / ConvertDistance.KilometersPerMile, 2), Math.Round(sample.SpeedMph, 2), "SpeedMph is wrong");

        }        

        [TestMethod]
        public void ReadFixedWidthSample()
        {
            var proivder = new CompuTrainerTXTFileProvider(fixedWidth);
            var samples = proivder.Samples.ToList();
            Assert.AreEqual(2, samples.Count, "wrong number of samples");

            var sample = samples[0];
            Assert.AreEqual(60, sample.HeartRateBpm, "HeartRateBpm is wrong");
            Assert.AreEqual((uint)47, sample.TimeMilisecondElapsed, "TimeMilisecondElapsed is wrong");
            Assert.AreEqual(22, sample.PowerWatts, "PowerWatts is wrong");
            Assert.AreEqual(1, sample.CadenceRpm, "CadenceRpm is wrong");
            Assert.AreEqual(0.0001, Math.Round(sample.DistanceKilometerElapsed, 4), "DistanceKilometerElapsed is wrong");
            Assert.AreEqual(Math.Round(5.79 / ConvertDistance.KilometersPerMile, 2), Math.Round(sample.SpeedMph, 2), "SpeedMph is wrong");

            sample = samples[1];
            Assert.AreEqual(80, sample.HeartRateBpm, "HeartRateBpm is wrong");
            Assert.AreEqual((uint)94, sample.TimeMilisecondElapsed, "TimeMilisecondElapsed is wrong");
            Assert.AreEqual(107, sample.PowerWatts, "PowerWatts is wrong");
            Assert.AreEqual(2, sample.CadenceRpm, "CadenceRpm is wrong");
            Assert.AreEqual(0.0002, Math.Round(sample.DistanceKilometerElapsed, 4), "DistanceKilometerElapsed is wrong");
            Assert.AreEqual(Math.Round(5.31 / ConvertDistance.KilometersPerMile, 2), Math.Round(sample.SpeedMph, 2), "SpeedMph is wrong");
        }        

        [TestMethod]
        public void ParseStartTimeFromFileName()
        {
            var provider = CreateProvider("Tom-325Watts_150W2_27_03_ERG8.erg-2013-12-07-12-25-30.cdf.txt");
            Assert.AreEqual(new DateTime(2013, 12, 07, 12, 25, 30), provider.StartTime, "start time is wrong");

            provider = CreateProvider("JAIME-40kTTFLAT.3DC-2012-06-09-16-34-32.CDF.txt");
            Assert.AreEqual(new DateTime(2012, 06, 09, 16, 34, 32), provider.StartTime, "start time is wrong");

            provider = CreateProvider("MARK LIVERSEDGE-MANERG-2010-09-29-12-34-03.CDF.txt");
            Assert.AreEqual(new DateTime(2010, 09, 29, 12, 34, 03), provider.StartTime, "start time is wrong");

            provider = CreateProvider("foo-2010-09-29-12-34-03.CDF.txt");
            Assert.AreEqual(new DateTime(2010, 09, 29, 12, 34, 03), provider.StartTime, "start time is wrong");

            provider = CreateProvider("2010-09-29-12-34-03.CDF.txt");
            Assert.AreEqual(new DateTime(2010, 09, 29, 12, 34, 03), provider.StartTime, "start time is wrong");
        }

        [TestMethod]
        public void ErrorOnNoDateInFilename()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => CreateProvider("Foo"));
            StringAssert.Contains(e.Message, "Joe Shmoe");
        }

        [TestMethod]
        public void ErrorOnMissingDatePartInFilename()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => CreateProvider("MARK LIVERSEDGE-MANERG--09-29-12-34-03.CDF.txt"));
            StringAssert.Contains(e.Message, "Joe Shmoe");
            
            e = ExceptionAssert.Throws<Exception>(() => CreateProvider("MARK LIVERSEDGE-MANERG-2010-29-12-34-03.CDF.txt"));
            StringAssert.Contains(e.Message, "Joe Shmoe");
        }

        [TestMethod]
        public void ErrorOnBadExtension()
        {
            Exception e = ExceptionAssert.Throws<Exception>(() => CreateProvider("MARK LIVERSEDGE-MANERG-2010-09-29-12-34-03.txt"));
            StringAssert.Contains(e.Message, "Joe Shmoe");

            e = ExceptionAssert.Throws<Exception>(() => CreateProvider("MARK LIVERSEDGE-MANERG-2010-09-29-12-34-03.CDF.txts"));
            StringAssert.Contains(e.Message, "Joe Shmoe");
        }

        CompuTrainerTXTFileProvider CreateProvider(string source)
        {
            SourcedStream stream = new SourcedStream()
            {
                Stream = Util.CreateStream(""),
                Source = source

            };
            return new CompuTrainerTXTFileProvider(stream);
        }
    }
}
