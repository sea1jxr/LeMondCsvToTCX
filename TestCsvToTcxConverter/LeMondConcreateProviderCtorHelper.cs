using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using ConvertToTcx;

namespace TestCsvToTcxConverter
{
    public class LeMondConcreateProviderCtorHelper
    {
        public string SourceName { get; set; }
        public TextFieldParser Parser { get; set; }
        public string[] FirstRow { get; set; }

        public LeMondConcreateProviderCtorHelper(SourcedStream reader)
        {
            SourceName = reader.Source;
            Parser = new TextFieldParser(reader.Stream);
            Parser.TextFieldType = FieldType.Delimited;
            Parser.Delimiters = new[] { "," };
            FirstRow = Parser.ReadFields();

        }
    }
}
