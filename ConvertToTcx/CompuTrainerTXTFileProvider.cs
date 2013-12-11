using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ConvertToTcx
{
    public class CompuTrainerTXTFileProvider : IComputrainerProvider
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
            int milesIndex = -1;
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
                kmIndex = GetIndex(labels, "KM", false);
                milesIndex = GetIndex(labels, "miles", false);
                if (kmIndex < 0 && milesIndex < 0)
                {
                    throw new Exception("didn't find KM or miles fields.  At least one of them must be present");
                }
            }

            int GetIndex(List<string> labels, string labelToFind, bool errorIfMissing = true)
            {
                int index = labels.IndexOf(labelToFind);
                if (errorIfMissing && index < 0)
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

                        if (line == null)
                        {
                            break;
                        }

                        var split = this.delimeterRegex.Split(line);
                        var valuesEnumeration = split.Where((s, i) => i % 2 == 0);
                        // in the space delemited case we get an empty string for a first value.
                        if (valuesEnumeration.First() == "")
                        {
                            valuesEnumeration = valuesEnumeration.Skip(1);
                        }
                        var values = valuesEnumeration.ToList();
                        

                        float distanceKilometerElapsed;
                        if (kmIndex > 0)
                        {
                            distanceKilometerElapsed = GetValue(values, kmIndex, Convert.ToSingle, "km");
                        }
                        else
                        {
                            distanceKilometerElapsed = GetValue(values, milesIndex, Convert.ToSingle, "miles");
                            distanceKilometerElapsed = ConvertDistance.MilesToKilometers(distanceKilometerElapsed);
                        }

                        yield return new ComputrainerDataSample()
                        {
                            TimeMilisecondElapsed = GetValue(values, msIndex, Convert.ToUInt32, "ms"),
                            HeartRateBpm = GetValue(values, hrIndex, Convert.ToInt32, "hr"),
                            CadenceRpm = GetValue(values, rpmIndex, Convert.ToInt32, "rpm"),
                            PowerWatts = GetValue(values, wattsIndex, (s) => Convert.ToInt32(Convert.ToSingle(s)), "watts"),
                            SpeedMph = GetValue(values, speedIndex, Convert.ToSingle, "speed") / ConvertDistance.KilometersPerMile,
                            DistanceKilometerElapsed = distanceKilometerElapsed
                        };

                        line = this.input.ReadLine();
                    }

                }
            }

            T GetValue<T>(List<string> values, int index, Func<string, T> parser, string fieldName)
            {
                string stringValue = "noval";
                try
                {
                    stringValue = values[index];
                    stringValue = Unquote(stringValue);
                    return parser(stringValue);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Error parsering field '{0}' with value '{1}'.", fieldName, stringValue), e);
                }
            }

            private string Unquote(string input)
            {
                if (input.Length >= 2 && input[0] == '"' && input[input.Length - 1] == '"')
                {
                    return input.Substring(1, input.Length - 2);
                }
                return input;
            }
        }
    }
}
