using System;
using System.IO;
using dokimi.Config;
using dokimi.Printers;
using dokimi.Printers.ConsoleApplication1;
using dokimi.core;

namespace dokimi
{
    public class RunSpecCommandPrinterEvaluator
    {
        public ISpecSuitePrinter GetCommandPrinter(PrintFormatInfo format, string destination, string assemblyName)
        {
            switch (format)
            {
                case PrintFormatInfo.Console:
                    return new ConsoleSpecSuitePrinter();
                case PrintFormatInfo.Word:
                    return new WordSpecSuitePrinter(Path.Combine(destination, assemblyName));
            }

            throw new NotSupportedException(string.Format("Print format {0} is not currently suppported.", format));
        }
    }
}