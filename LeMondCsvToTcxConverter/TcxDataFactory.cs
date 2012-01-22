using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConvertToTcx
{
    public class TcxDataFactory
    {
        private Func<SourcedStream, ITcxData> lemond;
        private Func<SourcedStream, ITcxData> computrainer;

        public TcxDataFactory(Func<SourcedStream, ITcxData> lemond, Func<SourcedStream, ITcxData> computrainer)
        {
            this.lemond = lemond;
            this.computrainer = computrainer;
        }

        public static TcxDataFactory CreateDefault()
        {
            Func<SourcedStream, ITcxData> lemond = (r) =>
                {
                    var provider = LeMondCsvDataProvider.Create(r);
                    var reader = new LeMondDataReader(provider);
                    return new LeMondTcxData(reader);
                
                };
            Func<SourcedStream, ITcxData> computrainer = (r) =>
                {
                    var provider = new CompuTrainer3DPFileProvider(r);
                    return new CompuTrainerTcxData(provider);
                };

            return new TcxDataFactory(lemond, computrainer);
        }
        
        public ITcxData Create(SourcedStream reader)
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
