using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConvertToTcx
{
    public class SourcedStream
    {
        public string Source { get; set; }
        public Stream Stream { get; set; }
    }
}
