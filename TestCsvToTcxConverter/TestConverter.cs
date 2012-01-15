using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LeMondCsvToTcxConverter;

namespace TestCsvToTcxConverter
{
    [TestClass]
    public class TestConverter
    {
        StringBuilder result = new StringBuilder();
        TextWriter textWriter;
        TextReader file1 = new StringReader(
@"LeMond,FW 1.00,HW 1.00,gforce,120102,16:31
TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
00:00:01,2.0,3.0,4,5,6,7,8,9
");

            TextReader file2 = new StringReader(
@"LeMond,FW 1.00,HW 1.00,gforce,120102,16:32
TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
00:00:01,10.0,11.0,12,13,14,15,16,17
");
        [TestInitialize]
        public void TestInitialize()
        {
            textWriter = new StringWriter(result);
        }

        [TestMethod]
        // this is a crapy baseline test, need to work on it.
        public void TestComboFile()
        {
            new Converter().WriteTcxFile(new TextReader[] { file1, file2 }, textWriter);
            string expected = 
@"<?xml version=""1.0"" encoding=""utf-16""?><TrainingCenterDatabase xmlns=""http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2""><Activities><Activity Sport=""Biking""><Id>2012-01-03T00:31:00Z</Id><Lap StartTime=""2012-01-03T00:31:00Z""><TotalTimeSeconds>1</TotalTimeSeconds><DistanceMeters>3000</DistanceMeters><Calories>7</Calories><Intensity>Active</Intensity><TriggerMethod>Manual</TriggerMethod><Track><Trackpoint><Time>2012-01-03T00:31:00Z</Time><DistanceMeters>3000</DistanceMeters><HeartRateBpm><Value>5</Value></HeartRateBpm><Cadence>6</Cadence><Extensions><TPX xmlns=""http://www.garmin.com/xmlschemas/ActivityExtension/v2""><Speed>0</Speed><Watts>4</Watts></TPX></Extensions></Trackpoint><Trackpoint><Time>2012-01-03T00:31:01Z</Time><DistanceMeters>3000</DistanceMeters><HeartRateBpm><Value>5</Value></HeartRateBpm><Cadence>6</Cadence><Extensions><TPX xmlns=""http://www.garmin.com/xmlschemas/ActivityExtension/v2""><Speed>0</Speed><Watts>4</Watts></TPX></Extensions></Trackpoint></Track></Lap><Lap StartTime=""2012-01-03T00:32:00Z""><TotalTimeSeconds>1</TotalTimeSeconds><DistanceMeters>14000</DistanceMeters><Calories>22</Calories><Intensity>Active</Intensity><TriggerMethod>Manual</TriggerMethod><Track><Trackpoint><Time>2012-01-03T00:32:00Z</Time><DistanceMeters>14000</DistanceMeters><HeartRateBpm><Value>13</Value></HeartRateBpm><Cadence>14</Cadence><Extensions><TPX xmlns=""http://www.garmin.com/xmlschemas/ActivityExtension/v2""><Speed>0</Speed><Watts>12</Watts></TPX></Extensions></Trackpoint><Trackpoint><Time>2012-01-03T00:32:01Z</Time><DistanceMeters>14000</DistanceMeters><HeartRateBpm><Value>13</Value></HeartRateBpm><Cadence>14</Cadence><Extensions><TPX xmlns=""http://www.garmin.com/xmlschemas/ActivityExtension/v2""><Speed>0</Speed><Watts>12</Watts></TPX></Extensions></Trackpoint></Track></Lap></Activity></Activities></TrainingCenterDatabase>";
            Assert.AreEqual(expected, result.ToString());
        }
    }
}
