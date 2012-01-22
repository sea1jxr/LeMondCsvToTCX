using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ConvertToTcx
{
    /// <summary>
    /// 
    /// Sample File:
    /// LeMond,FW 1.00,HW 1.00,gforce,120102,16:31
    /// TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
    /// 00:00:01,2.0,3.0,4,5,6,7,8,9
    /// </summary>
    public class LeMondGForceCsvDataProvider : LeMondCsvDataProvider
    {
        public LeMondGForceCsvDataProvider(string sourceName, TextFieldParser parser, string [] firstRow)
            :base(parser)
        {
            if (firstRow.Length < 6)
            {
                throw new Exception(string.Format("Invalid gforce header. Header = '{0}'", string.Join(",", firstRow)));
            }

            int year, month, day;
            ParseDate(firstRow[4], out year, out month, out day);
            int hour, minute;
            ParseTime(firstRow[5], out hour, out minute);
            StartTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Local);

            if (Parser.EndOfData)
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it is missing the data field headers.", sourceName));
            }

            firstRow = Parser.ReadFields();
            if (!(firstRow.Length >= 7 &&
                  firstRow[0] == "TIME" &&
                  firstRow[1] == "SPEED" &&
                  firstRow[2] == "DIST" &&
                  firstRow[3] == "POWER" &&
                  firstRow[4] == "HEART RATE" &&
                  firstRow[5] == "RPM" &&
                  firstRow[6] == "CALORIES"))
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it does not contain the correct data fields.", sourceName));
            }
        }

        public static void ParseDate(string fileDate, out int year, out int month, out int day)
        {
            if (fileDate == null || fileDate.Length != 6 || 
                !(
                   int.TryParse(fileDate.Substring(0, 2), out year) &&
                   int.TryParse(fileDate.Substring(2, 2), out month) &&
                   int.TryParse(fileDate.Substring(4, 2), out day)
                 )
               )
            {
                throw new Exception("The start date is not in the correct format, it is expected to be in a 'YYMMDD' format");
            }
            
            // assuming 21st century....
            year += 2000;
        }

        public static void ParseTime(string time, out int hour, out int minute)
        {
            if (time == null || time.Length != 5 || time.IndexOf(':') != 2)
            {
                throw new Exception("The start time is not in the correct format, it is expected to be in a 'HH:MM' format");
            }

            if (!(int.TryParse(time.Substring(0, 2), out hour) && int.TryParse(time.Substring(3, 2), out minute)))
            {
                throw new Exception("The start time is not in the correct format, it is expected to be in a 'HH:MM' format where the HH and MM are integers");
            }
        }
    }
}
