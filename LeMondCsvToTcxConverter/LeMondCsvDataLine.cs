using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeMondCsvToTcxConverter
{
    public class LeMondCsvDataLine
    {
        public string Time { get; set; }
        public string Speed { get; set; }
        public string Distance { get; set; }
        public string Power { get; set; }
        public string HeartRate { get; set; }
        public string Rpm { get; set; }
        public string Calories { get; set; }
    }
}
