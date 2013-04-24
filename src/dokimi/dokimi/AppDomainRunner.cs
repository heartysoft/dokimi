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

                var command = new RunSpecCommand(file, config.Formatters.IncludePath, config.Formatters.Formatters, config.Print);

                runner.DescribeSpec(command);
            }
        }
    }

    public class AppDomainRunner : MarshalByRefObject
    {
        public void DescribeSpec(RunSpecCommand command)
        {
            var formatters = ExtractMessageFormatters(command.FormattersPath);

            var assembly = Assembly.LoadFrom(command.AssemblyPath);
            var specExtractor = new SpecExtractor();

            RegisterFormatters(formatters, specExtractor, command);
            
            var specs = specExtractor.ExtractSuite(assembly);
            var assemblyName = Path.GetFileNameWithoutExtension(command.AssemblyPath);
            
            var printer = new RunSpecCommandPrinterEvaluator().GetCommandPrinter(command.PrintInfo.Format, command.PrintInfo.Destination, assemblyName);
            printer.Print(specs);
        }
        
        private static IEnumerable<MessageFormatter> ExtractMessageFormatters(string path)
        {
            var extractedFormatters = new List<MessageFormatter>();

            if (Directory.Exists(path))
            {
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
}