using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using dokimi.Config;
using dokimi.core;
using dokimi.core.Attributes;

namespace dokimi
{
    public class RunnerHelper
    {
        private const string AssemblyFilesPattern = "*.exe,*.dll";

        public static string[] GetFileList(string path)
        {
            return Directory
                .GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => AssemblyFilesPattern.Contains(Path.GetExtension(s).ToLower()))
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

                var command = new RunSpecCommand(file, config.Formatters.IncludePath, config.Formatters.Formatters);

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

            RegisterFormatters(formatters, specExtractor, command);
            
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
                                                    .Where(x => typeof (MessageFormatter).IsAssignableFrom(x) 
                                                        && !x.IsAbstract
                                                        && x.GetCustomAttribute<MessageFormatterTypeKeyAttribute>() != null)
                                                    .ToArray();

                extractedFormatters.AddRange(messageFormatterTypes.Select(Activator.CreateInstance)
                                                                  .Select(x => x)
                                                                  .Cast<MessageFormatter>());
            }

            return extractedFormatters;
        }

        private static void RegisterFormatters(IEnumerable<MessageFormatter> formatters, SpecExtractor specExtractor, RunSpecCommand command)
        {
            var requiredFormatters = formatters
                .Select(
                    x =>
                    new {Attribute = x.GetType().GetCustomAttribute<MessageFormatterTypeKeyAttribute>(), Formatter = x})
                .Where(x => x.Attribute != null)
                .Where(x => command.Formatters.Any(y => y.Type == x.Attribute.Type))
                .Select(x => new
                    {
                        x.Formatter,
                        Categories =
                                 command.Formatters.Where(y => y.Type == x.Attribute.Type)
                                        .Select(y => new SpecificationCategory(y.ContextName, y.CategoryName))
                                        .ToArray()
                    })
                .ToArray();

            foreach (var entry in requiredFormatters)
            {
                foreach (var category in entry.Categories)
                {
                    specExtractor.RegisterFormatter(category, entry.Formatter);
                }
            }
        }
    }

    [Serializable]
    public class RunSpecCommand
    {
        public string AssemblyPath { get; private set; }
        public string FormattersPath { get; private set; }
        public FormatterInfo[] Formatters { get; private set; }

        public RunSpecCommand(string assemblyPath, string formattersPath, IEnumerable<FormatterInfo> formatters)
        {
            AssemblyPath = assemblyPath;
            FormattersPath = formattersPath;
            Formatters = formatters.ToArray();
        }
    }

    
}