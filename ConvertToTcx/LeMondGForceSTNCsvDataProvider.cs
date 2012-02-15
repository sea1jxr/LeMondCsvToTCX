using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace ConvertToTcx
{
    /// <summary>
    /// 
    /// Sample File:
    /// LeMond,FW 0.25,HW 1.0,STN,111230,15:02,,,
    /// TIME,SPEED,DIST,POWER,HEART RATE,RPM,CALORIES,TORQUE,TARGET HR
    /// 0:00:01,22.3,0,219,0,99,0,21,0
    /// </summary>
    public class LeMondGForceSTNCsvDataProvider : LeMondCsvDataProvider
    {
        // default to the 0.25 version
        private double firmwareVersion = 0.25;

        public LeMondGForceSTNCsvDataProvider(string sourceName, TextFieldParser parser, string[] firstRow)
            :base(parser)
        {

            if (firstRow.Length < 6)
            {
                throw new Exception(string.Format("Invalid gforce STN header. Header = '{0}'", string.Join(",", firstRow)));
            }

            firmwareVersion = LeMondGForceSTNCsvDataProvider.ParseFirmwareVersion(firstRow[1]);

            int year, month, day;
            LeMondGForceCsvDataProvider.ParseDate(firstRow[4], out year, out month, out day);
            int hour, minute;
            LeMondGForceCsvDataProvider.ParseTime(firstRow[5], out hour, out minute);
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

        private static double ParseFirmwareVersion(string firmwareVersionValue)
        {
         
            double version;
            if (firmwareVersionValue.Length < 7 ||
                firmwareVersionValue.Substring(0, 3) != "FW " ||
                !double.TryParse(firmwareVersionValue.Substring(3), out version))
            {
                throw new Exception(string.Format("The firmware version was not in the correct format, expected 'FW 0.00' and got '{0}'", firmwareVersionValue));
            }

            return version;
        }

        public override double ConvertDistanceToKilometers(double distance)
        {
            if (ContainsEnglishUnits)
            {
                return ConvertDistance.MilesToKilometers(distance);
            }

            return distance;
        }

        public override double ConvertSpeedToKilometersPerHour(double speed)
        {
            if (ContainsEnglishUnits)
            {
                // m/h * k/m
                return ConvertDistance.MilesToKilometers(speed);
            }

            return speed;
        }

        private bool ContainsEnglishUnits
        {
            get { return firmwareVersion < 0.31; }
        }
    }
}
