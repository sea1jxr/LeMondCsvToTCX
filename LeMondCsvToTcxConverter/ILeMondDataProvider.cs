using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeMondCsvToTcxConverter
{
    public interface ILeMondDataProvider
    {
        string StartDate { get; }
        string StartTime { get; }
        IEnumerable<LeMondCsvDataLine> DataLines { get; }
    }
}
