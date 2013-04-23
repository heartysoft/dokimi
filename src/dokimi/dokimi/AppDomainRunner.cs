using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using dokimi.Config;
using dokimi.core;

namespace dokimi
{
    public class RunnerHelper
    {
        public const string AssemblyFilesPattern = "*.exe,*.dll";

        public static string[] GetFileList(string path)
        {
            return Directory
                .GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => RunnerHelper.AssemblyFilesPattern.Contains(Path.GetExtension(s).ToLower()))
                .ToArray();
        }
    }

    public class SpecRunner
    {
        public void DescribeAssembly(DokimiConfig config)
        {
            var files = RunnerHelper.GetFileList(config.Source.IncludePath);

            foreach (var file in files)
            {
                var appDomain = AppDomain.CreateDomain(file, AppDomain.CurrentDomain.Evidence);
                var runner = (AppDomainRunner)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, (typeof (AppDomainRunner)).FullName);

                var command = new RunSpecCommand(file, config.Formatters.IncludePath);

                var res = runner.DescribeSpec(command);
            }
        }
    }

    public class AppDomainRunner : MarshalByRefObject
    {
        public string DescribeSpec(RunSpecCommand command)
        {
            var formatters = ExtractMessageFormatters(command.FormattersPath);

            var assembly = Assembly.LoadFrom(command.AssemblyPath);
            var specExtractor = new SpecExtractor();

            RegisterFormatters(formatters, specExtractor);
            
            //.RegisterFormatter(new SpecificationCategory("Accounts", "Overdrafts"), new DefaultFormatter());
            //var specs = specExtractor.ExtractSuite(assembly);
            
            var specs = specExtractor.RunSuite(assembly);

            var printer = new SpecSuiteConsolePrinter();

            printer.Print(specs);

            return DateTime.Now.ToString();
        }

        private static IList<MessageFormatter> ExtractMessageFormatters(string path)
        {
            var extractedFormatters = new List<MessageFormatter>();
            var formatterFiles = RunnerHelper.GetFileList(path);

            foreach (var file in formatterFiles)
            {
                var assembly = Assembly.LoadFrom(file);
                var messageFormatterTypes = assembly.GetTypes()
                                                    .Where(x => typeof (MessageFormatter).IsAssignableFrom(x) && !x.IsAbstract)
                                                    .ToArray();

                extractedFormatters.AddRange(messageFormatterTypes.Select(Activator.CreateInstance)
                                                                  .Select(x => x)
                                                                  .Cast<MessageFormatter>());
            }

            return extractedFormatters;
        }

        private static void RegisterFormatters(IList<MessageFormatter> formatters, SpecExtractor specExtractor)
        {
            foreach (var formatter in formatters)
            {
                specExtractor.RegisterFormatter(new SpecificationCategory("test", "test"), formatter);
            }
        }
    }

    [Serializable]
    public class RunSpecCommand
    {
        public string AssemblyPath { get; private set; }
        public string FormattersPath { get; private set; }

        public RunSpecCommand(string assemblyPath, string formattersPath)
        {
            AssemblyPath = assemblyPath;
            FormattersPath = formattersPath;
        }
    }
}