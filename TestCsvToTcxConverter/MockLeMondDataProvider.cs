﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeMondCsvToTcxConverter;

namespace TestCsvToTcxConverter
{
    class MockLeMondDataProvider : ILeMondDataProvider
    {
        public DateTime StartTime { get; set; }

        public IEnumerable<LeMondCsvDataLine> DataLines { get; set; }
    }
}
