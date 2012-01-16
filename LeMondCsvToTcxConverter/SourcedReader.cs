using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LeMondCsvToTcxConverter
{
    public class SourcedReader
    {
        public string Source { get; set; }
        public TextReader TextReader { get; set; }
    }
}
