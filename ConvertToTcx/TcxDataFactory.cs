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
        private Func<SourcedStream, ITcxData> computrainer3DP;
        private Func<SourcedStream, ITcxData> computrainerTXT;

        public TcxDataFactory(
            Func<SourcedStream, ITcxData> lemond, 
            Func<SourcedStream, ITcxData> computrainer3DP,
            Func<SourcedStream, ITcxData> computrainerTXT)

        {
            this.lemond = lemond;
            this.computrainer3DP = computrainer3DP;
            this.computrainerTXT = computrainerTXT;
        }

        public static TcxDataFactory CreateDefault()
        {
            Func<SourcedStream, ITcxData> lemond = (r) =>
                {
                    var provider = LeMondCsvDataProvider.Create(r);
                    var reader = new LeMondDataReader(provider);
                    return new LeMondTcxData(reader);
                
                };
            Func<SourcedStream, ITcxData> computrainer3DP = (r) =>
                {
                    var provider = new CompuTrainer3DPFileProvider(r);
                    return new CompuTrainerTcxData(provider);
                };
            Func<SourcedStream, ITcxData> computrainerTXT = (r) =>
            {
                var provider = new CompuTrainerTXTFileProvider(r);
                return new CompuTrainerTcxData(provider);
            };

            return new TcxDataFactory(lemond, computrainer3DP, computrainerTXT);
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
                // CompuTrainer .3DP
                return computrainer3DP(reader);
            }
            else if (reader.Source.EndsWith(".cdf.txt", StringComparison.OrdinalIgnoreCase))
            {
                return computrainerTXT(reader);
            }

            throw new Exception(string.Format("The extension '{0}' is not a supported file type", extension));
        }
    }
}
