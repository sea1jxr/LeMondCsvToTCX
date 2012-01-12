using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace LeMondCsvToTcxConverter
{
    public class FileLeMondCsvDataProvider : ILeMondDataProvider
    {
        private string startData;
        private string startTime;
        private TextFieldParser parser;

        public FileLeMondCsvDataProvider(string fileName)
        {
            parser = new TextFieldParser(fileName);
            parser.TextFieldType = FieldType.Delimited;
            parser.Delimiters = new [] { "," };
            if (parser.EndOfData)
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it is empty.", fileName));
            }

            var row = parser.ReadFields();
            if (!(row.Length >= 6 && row[0] == "LeMond"))
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it doesn't say 'LeMond' in the first field.", fileName));
            }

            startData = row[4];
            startTime = row[5];

            if (parser.EndOfData)
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it is missing the data field headers.", fileName));
            }

            row = parser.ReadFields();
            if(! (row.Length >= 7 &&
                  row[0] == "TIME" &&
                  row[1] == "SPEED" &&
                  row[2] == "DIST" &&
                  row[3] == "POWER" &&
                  row[4] == "HEART RATE" &&
                  row[5] == "RPM" &&
                  row[6] == "CALORIES"))
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it does not contain the correct data fields.", fileName));
            }
        }

        public string StartDate
        {
            get { return startData; }
        }

        public string StartTime
        {
            get { return startTime; }
        }

        public IEnumerable<LeMondCsvDataLine> DataLines
        {
            get 
            {
                while (!parser.EndOfData)
                {
                    var row = parser.ReadFields();
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
            }
        }
    }
}
