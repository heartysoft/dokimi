using System;
using System.Reflection;
using System.Security.Policy;
using SimpleConfig;
using dokimi.core;

namespace dokimi
{
    public class AppDomainRunner : MarshalByRefObject
    {
        public string DescribeSpec(string path)
        {
            Console.WriteLine(path);
            Console.WriteLine("Runner: {0}", AppDomain.CurrentDomain.FriendlyName);
            //Assembly.Load() -> this will load into this app domain, not parent app domain.

            const string file = @"C:\UnoGit\CodeTF\Twang\Solutions\Twang\UnitTests\bin\Debug\UnitTests.dll";
            var assembly = Assembly.LoadFrom(file);
            var specExtractor = new SpecExtractor().RegisterFormatter(new SpecificationCategory("Accounts", "Overdrafts"), new DefaultFormatter());
            //var specs = specExtractor.ExtractSuite(assembly);
            var specs = specExtractor.RunSuite(assembly);

            var printer = new SpecSuiteConsolePrinter();

            printer.Print(specs);


            return DateTime.Now.ToString();
        }
    }

    public class SpecRunner
    {
        public void DescribeAssembly(string path)
        {
            var appDomain = AppDomain.CreateDomain(path, AppDomain.CurrentDomain.Evidence);
            var runner = (AppDomainRunner)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, (typeof (AppDomainRunner)).FullName);
            var res = runner.DescribeSpec(path);

            Console.WriteLine("Res: {0}", res);
        }
    }


}