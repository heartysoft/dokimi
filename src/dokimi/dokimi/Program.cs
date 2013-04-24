using System;
using System.Collections.Generic;
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
            ConfigurationHelper.VerifyAndSetDefaultsIfNeeded(config);

            OptionSet parser;
            if (!ParseCommandParams(args, config, out parser))
            {
                ShowHelp(parser);
                return;
            }

            try
            {
                var runner = new SpecRunner();
                runner.DescribeAssembly(config);
            }
            catch (Exception e)
            {
                Console.WriteLine("There is a problem with the execution:");
                Console.WriteLine(e.Message);
            }
        }

        private static bool ParseCommandParams(IEnumerable<string> args, DokimiConfig config, out OptionSet parser)
        {
            var success = true;

            parser = new OptionSet
                {
                    {"sp=|sourcePath=", "Source path for the test libraries.", value => config.Source.IncludePath = value},
                    {
                        "pl=|printLevel=", "Print level (Minimal|Verbose). Default is Minimal.", pl =>
                            {
                                PrintLevelInfo printLevel;
                                if (Enum.TryParse(pl, out printLevel))
                                    config.Print.Level = printLevel;
                            }
                    },
                    {
                        "pf=|printFormat=", "Print format (Console|Word). Default is Console.", pf =>
                            {
                                PrintFormatInfo printFormat;
                                if (Enum.TryParse(pf, out printFormat))
                                    config.Print.Format = printFormat;
                            }
                    },
                    {"pd=|printDestination", @"Print destination file path in (Word). Default \out.", pd => config.Print.Destination = pd },
                    {
                        "fp=|formattersPath=", @"Source path for the test message formatters. (if not provided, config is used. If not present in config, \formatters is used.",
                        path => config.Formatters.IncludePath = path
                    },
                    {"h|?|help", v => success = v == null},
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
                success = false;
            }

            return success;
        }

        private static void ShowHelp(OptionSet parser)
        {
            Console.WriteLine("Usage: dokimi.exe  [--sp=Source Path] [--pl=Print Level] [--pf=Print Format] [--fp=Formatters Path].");
            Console.WriteLine("Options:");
            parser.WriteOptionDescriptions(Console.Out);
        }
    }
}
