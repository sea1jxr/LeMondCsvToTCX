using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace ConvertToTcx
{

    /// <summary>
    /// 
    /// Sample File:
    /// LeMond, Revolution,FW 50,HW 02,30-Mar,18:33:17,Alt 30,Temp 25,Hum 45,Tire 2105,CF 150
    /// TIME,SPEED,DIST,POWER,HEART RATE,CADENCE,CALORIES,TARGET,,,
    /// 0:00:01,0,0,0,72,0,0,0,,,
    /// or
    /// LeMond, Revolution,FW 50,HW 02,08/20,16:48:28,Alt 1432,Temp 25,Hum 46,Tire 2097,CF 150
    /// TIME,SPEED,DIST,POWER,HEART RATE,CADENCE,CALORIES,TARGET
    /// 0:00:01,017.9,000.0,0060,000,071,0000,180<PWR<133,

    /// </summary>
    public class LeMondRevolutionCsvDataProvider : LeMondCsvDataProvider
    {

        public LeMondRevolutionCsvDataProvider(string sourceName, TextFieldParser parser, string[] firstRow)
            :base(parser)
        {
            if (firstRow.Length < 6)
            {
                throw new Exception(string.Format("Invalid Revolution header. Header = '{0}'", string.Join(",", firstRow)));
            }

            int year, month, day;
            ParseDate(firstRow[4], out year, out month, out day);
            int hour, minute, second;
            ParseTime(firstRow[5], out hour, out minute, out second);
            StartTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);

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
                  firstRow[5] == "CADENCE" &&
                  firstRow[6] == "CALORIES"))
            {
                throw new Exception(string.Format("The file {0} does not seem to be a valid LeMond .csv file because it does not contain the correct data fields.", sourceName));
            }
        }

        static readonly string[] months = { 
                                     "Jan",
                                     "Feb",
                                     "Mar",
                                     "Apr",
                                     "May",
                                     "Jun",
                                     "Jul",
                                     "Aug",
                                     "Oct",
                                     "Sep",
                                     "Nov",
                                     "Dec"
                                 };
        
        public static bool TryGetMonth(string abbreviation, out int month)
        {
            for (int i = 0; i < months.Length; i++)
            {
                if (months[i] == abbreviation)
                {
                    month =  i + 1;
                    return true;
                }
            }

            month = 0;
            return false;
        }

        public static void ParseDate(string date, out int year, out int month, out int day)
        {
            DateTime dateTime;
            if (!DateTime.TryParse(date, out dateTime))
            {
                throw new Exception("The start date is not in the correct format, it is expected to be in a 'DD-MMM' or 'MM/DD' format");
            }
            
            var now = DateTime.Now;
            year = now.Year;
            month = dateTime.Month;
            day = dateTime.Day;

            if (dateTime.Year == now.Year && now.Month < month)
            {
                // a year wasn't provided, and 
                // since we aren't to this month
                // we will assume it was last year.
                year -= 1;
            }
        }

        public static void ParseTime(string time, out int hour, out int minute, out int sec)
        {
            if (time == null)
            {
                throw new Exception("Unable to find a time value");
            }

            int hourMinuteSeperator = time.IndexOf(':');
            int minuteSecondSeperator = time.LastIndexOf(':');
            if (time.Length < 7 || time.Length > 8  || (hourMinuteSeperator != 1 && hourMinuteSeperator != 2)  || (minuteSecondSeperator != 4 && minuteSecondSeperator != 5))
            {
                throw new Exception("The start time is not in the correct format, it is expected to be in a '[H]H:MM:SS' format");
            }

            if (!(int.TryParse(time.Substring(0, hourMinuteSeperator), out hour) && 
                  int.TryParse(time.Substring(hourMinuteSeperator + 1, 2), out minute) &&
                  int.TryParse(time.Substring(minuteSecondSeperator + 1, 2), out sec)
                 )
               )
            {
                throw new Exception("The start time is not in the correct format, it is expected to be in a '[H]H:MM:SS' format where the HH and MM are integers");
            }
        }
    }
}
