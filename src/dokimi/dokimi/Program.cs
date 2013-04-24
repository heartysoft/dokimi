using System;
using NDesk.Options;
using SimpleConfig;
using dokimi.Config;

namespace dokimi
{
    /// <summary>
    /// dokimi
    //src
    //|- dokimi.core (dll)
    //     |- something to hold the core [Specification, StepInfo, SpecInfo, SpecExtractor, SpecSuite, SpecCategory, VerificationInput, Expectation, ExpectationFailedException, MessageFormatter, ISpecSuitePrinter] [independent of nunit]
    //|- dokimi.nunit (dll)[SpecificationTest, Ignore]
    //|- dokimi.exe (exe) [Word, Pdf etc. printers, running code, printing code]

    //dokimi.exe -root "/foo" -files="x.dll, y.dll" -printer "Word" -printparams "Path" -formatters="accountsformatter.dll, xyzformatter.dll" -printLevel "Verbose|Minimal"

    //Twang
    //|-Twang.Testing
    //|---GivenEventsStep, WhenCommandStep, EventBasedTestBase
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private void Run(string[] args)
        {
            var config = Configuration.Load<DokimiConfig>();

            bool show_help = false;

            var parser = new OptionSet
                {
                    {"sp=|sourcePath=", "Source path for the test libraries.", value => config.Source.IncludePath = value },
                    {
                        "pl=|printLevel=", "Print level (Minimal|Verbose).", pl =>
                            {
                                PrintLevelInfo printLevel;
                                if (Enum.TryParse(pl, out printLevel))
                                    config.Print.Level = printLevel;
                            }
                    },
                    {
                        "pf=|printFormat=", "Print format (Console|Word).", pl =>
                            {
                                PrintFormatInfo printFormat;
                                if (Enum.TryParse(pl, out printFormat))
                                    config.Print.Format = printFormat;
                            }
                    },
                    {
                        "fp=|formattersPath=", "Source path for the test message formatters.",
                        path => config.Formatters.IncludePath = path
                    },
                    { "h|?|help", v => show_help = v != null },
                };

            try
            {
                parser.Parse(args);
            }
            catch (OptionException optionException)
            {
                Console.Write("dokimi: ");
                Console.WriteLine(optionException.Message);
                Console.WriteLine("Try `dokimi --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(parser);
                return;
            }

            var runner = new SpecRunner();
            runner.DescribeAssembly(config);

            Console.ReadLine();
        }

        private static void ShowHelp(OptionSet parser)
        {
            Console.WriteLine("Usage: dokimi [OPTIONS]");
            Console.WriteLine("Default configuration is used for the specific [OPTIONS] if not provided.");
            Console.WriteLine("Options:");
            parser.WriteOptionDescriptions(Console.Out);
        }
    }
}
