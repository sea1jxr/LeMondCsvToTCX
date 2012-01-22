using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConvertToTcx
{
    public static class Util
    {
        public static Stream CreateStream(string s)
        {
            return new MemoryStream(Encoding.Default.GetBytes(s));
        }
    }
}
