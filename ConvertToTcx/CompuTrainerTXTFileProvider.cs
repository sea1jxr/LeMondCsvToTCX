using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ConvertToTcx
{
    public class CompuTrainerTXTFileProvider 
    {
        private StreamReader input;
        private string userName;
        private int age;
        private float weightKilos;
        private int upperHeartRate;
        private int lowerHeartRate;
        private DateTime startTime;

        private int numberOfDataPoints = -1;
        CompuTrainerTxtFieldParser dataParser;

        public CompuTrainerTXTFileProvider(SourcedStream sourced)
        {
            this.input = new StreamReader(sourced.Stream, Encoding.ASCII, true);

            ParseStartTimeFromFileName(sourced.Source);
            ReadHeaderInfomation();
            
        }

        private void ReadHeaderInfomation()
        {
            Regex numberOfRecordsLine = new Regex(@"^number of records = (\d+)");
            string line = this.input.ReadLine();
            while(line != null)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    // nothing to do 
                    // just let the next line be read
                }
                else if (this.numberOfDataPoints < 0)
                {
                    var match = numberOfRecordsLine.Match(line);
                    if (match.Success)
                    {
                        this.numberOfDataPoints = Convert.ToInt32(match.Groups[1].Value);
                    }
                }
                else
                {
                    this.dataParser = new CompuTrainerTxtFieldParser(this.input, line);
                    break;
                }

                line = this.input.ReadLine();
            }
        }

        private void ParseStartTimeFromFileName(string source)
        {
            var regex = new Regex(@"^.*[^-]*-?[^-]*-?(\d\d\d\d)-(\d\d)-(\d\d)-(\d\d)-(\d\d)-(\d\d)\.cdf.txt$", RegexOptions.IgnoreCase);
            var match = regex.Match(source);
            if (!match.Success)
            {
                throw new Exception("Unable to get the start time from the file name, please use the standard file name format (eg Joe Shmoe-SufferMuchErg-2011-10-18-11-36-33.CDF.txt)");
            }

            this.startTime = new DateTime(
                Convert.ToInt32(match.Groups[1].Value),
                Convert.ToInt32(match.Groups[2].Value),
                Convert.ToInt32(match.Groups[3].Value),
                Convert.ToInt32(match.Groups[4].Value),
                Convert.ToInt32(match.Groups[5].Value),
                Convert.ToInt32(match.Groups[6].Value));
        }

        public DateTime StartTime { get { return startTime; } }
        public int Age { get { return age; } }
        public string UserName { get { return userName; } }
        public float WeightKilos { get { return weightKilos; } }
        public int UpperHeartRate { get { return upperHeartRate; } }
        public int LowerHeartRate { get { return lowerHeartRate; } }

        public int SampleCount { get { return numberOfDataPoints; } }

        public IEnumerable<ComputrainerDataSample> Samples
        {
            get
            {
                return this.dataParser.Samples;
            }
        }

        class CompuTrainerTxtFieldParser
        {
            Regex delimeterRegex = new Regex("( +|,)");
            int msIndex = -1;
            int wattsIndex = -1;
            int rpmIndex = -1;
            int speedIndex = -1;
            int hrIndex = -1;
            int kmIndex = -1;
            StreamReader input;

            public CompuTrainerTxtFieldParser(StreamReader input, string dataHeaderLine)
            {
                this.input = input;

                var labels = this.delimeterRegex.Split(dataHeaderLine).Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
                msIndex = GetIndex(labels, "ms");
                wattsIndex = GetIndex(labels, "watts");
                rpmIndex = GetIndex(labels, "rpm");
                speedIndex = GetIndex(labels, "speed");
                hrIndex = GetIndex(labels, "hr");
                kmIndex = GetIndex(labels, "KM");
            }

            int GetIndex(List<string> labels, string labelToFind)
            {
                int index = labels.IndexOf(labelToFind);
                if (index < 0)
                {
                    throw new Exception(string.Format("File is missing the '{0}' data field", labelToFind));
                }
                return index;
            }

            public IEnumerable<ComputrainerDataSample> Samples
            {
                get
                {
                    string line = this.input.ReadLine();
                    while (line != null)
                    {
                        while (line != null && line.Trim() == "")
                        {
                            line = this.input.ReadLine();
                        }

                        var values = this.delimeterRegex.Split(line).Where(s => !String.IsNullOrWhiteSpace(s)).ToList();
                        yield return new ComputrainerDataSample()
                        {
                            TimeMilisecondElapsed = Convert.ToUInt32(values[msIndex]),
                            HeartRateBpm = Convert.ToInt32(values[hrIndex]),
                            CadenceRpm = Convert.ToInt32(values[rpmIndex]),
                            PowerWatts = Convert.ToInt32(values[wattsIndex]),
                            SpeedMph = Convert.ToSingle(values[speedIndex])/ConvertDistance.KilometersPerMile,
                            DistanceKilometerElapsed = Convert.ToSingle(values[kmIndex])
                        };

                        line = this.input.ReadLine();
                    }

                }

            }
        }
    }
}
