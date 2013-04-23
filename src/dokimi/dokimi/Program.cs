﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            new Program().Run();
        }

        private void Run()
        {
            var config = Configuration.Load<DokimiConfig>();

            var runner = new SpecRunner();
            runner.DescribeAssembly(config);

            //Console.WriteLine("Main: {0}", AppDomain.CurrentDomain.FriendlyName);

            Console.ReadLine();
        }
    }
}
