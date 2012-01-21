using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LeMondCsvToTcxConverter
{
    public class TcxDataFactory
    {
        private Func<SourcedReader, ITcxData> lemond;
        private Func<SourcedReader, ITcxData> computrainer;

        public TcxDataFactory(Func<SourcedReader, ITcxData> lemond, Func<SourcedReader, ITcxData> computrainer)
        {
            this.lemond = lemond;
            this.computrainer = computrainer;
        }
        
        public ITcxData Create(SourcedReader reader)
        {
            string extension = Path.GetExtension(reader.Source);
            if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                // LeMond
                return lemond(reader);
            }
            else if (extension.Equals(".3dp", StringComparison.OrdinalIgnoreCase))
            {
                // CompuTrainer
                return computrainer(reader);
            }

            throw new Exception(string.Format("The extension '{0}' is not a supported file type", extension));
        }
    }
}
