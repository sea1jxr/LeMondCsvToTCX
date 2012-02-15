using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace ConvertToTcx
{
    public abstract class LeMondCsvDataProvider : ILeMondDataProvider
    {
        private DateTime startTime;
        private TextFieldParser parser;

        public LeMondCsvDataProvider(TextFieldParser parser)
        {
            this.parser = parser;
        }

        public DateTime StartTime
        {
            get { return startTime; }
            protected set { startTime = value; }
        }

        public TextFieldParser Parser
        {
            get { return parser; }
        }

        public static ILeMondDataProvider Create(SourcedStream sourcedStream)
        {
            
            var parser = new TextFieldParser(sourcedStream.Stream);
            parser.TextFieldType = FieldType.Delimited;
            parser.Delimiters = new[] { "," };
            if (parser.EndOfData)
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it is empty.", sourcedStream.Source));
            }

            var row = parser.ReadFields();
            if (!(row.Length >= 1 && row[0] == "LeMond"))
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it doesn't say 'LeMond' in the first field.", sourcedStream.Source));
            }


            if (row.Length >= 4 && row[3] == "gforce")
            {
                return new LeMondGForceCsvDataProvider(sourcedStream.Source, parser, row);
            }
            if (row.Length >= 4 && row[3] == "STN")
            {
                return new LeMondGForceSTNCsvDataProvider(sourcedStream.Source, parser, row);
            }
            else if (row.Length >= 2 && row[1] == "Revolution")
            {
                return new LeMondRevolutionCsvDataProvider(sourcedStream.Source, parser, row);
            }

            throw new Exception(string.Format("Not a recognized LeMond device. Header = '{0}'", string.Join(",", row)));
            
        }

        public virtual double ConvertSpeedToKilometersPerHour(double speed)
        {
            return speed;
        }

        public virtual double ConvertDistanceToKilometers(double distance)
        {
            return distance;
        }

        public IEnumerable<LeMondCsvDataLine> DataLines
        {
            get
            {
                int rowCount = 0;
                string [] row = null;
                // the LeMond devices tend to cut off data on the final
                // line of data, that is ok, but we will error if data is
                // missing in the middle of the file.
                bool previousLineWasMissingColumns = false;
                while (!Parser.EndOfData)
                {
                    if (previousLineWasMissingColumns)
                    {
                        throw new Exception(string.Format("Missing data on row {0}.  Only {1} field(s) were found of the 7 needed. \r\nRow Data: {2}", rowCount, row.Length, string.Join(",", row)));
                    }

                    row = Parser.ReadFields();
                    rowCount++;
                    if (row.Length >= 7)
                    {
                        var data = new LeMondCsvDataLine()
                        {
                            Time = row[0],
                            Speed = row[1],
                            Distance = row[2],
                            Power = row[3],
                            HeartRate = row[4],
                            Rpm = row[5],
                            Calories = row[6]
                        };
                        yield return data;
                    }
                    else
                    {
                        previousLineWasMissingColumns = true;
                    }
                }
            }
        }

    }
}
