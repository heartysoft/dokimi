using System;
using System.IO;
using System.Reflection;
using dokimi.Config;

namespace dokimi
{
    public static class ConfigurationHelper
    {
        private const string DEFAULT_SRC_PATH = "src";
        private const string DEFAULT_PRINT_DEST_PATH = "out";
        private const string DEFAULT_FORMATTERS_PATH = "formatters";
        private const PrintFormatInfo DEFAULT_PRINT_FORMAT = PrintFormatInfo.Console;
        private const PrintLevelInfo DEFAULT_PRINT_LEVEL = PrintLevelInfo.Minimal;

        public static void VerifyAndSetDefaultsIfNeeded(DokimiConfig config)
        {
            if (config.Print == null)
                config.Print = new PrintInfo { Format = DEFAULT_PRINT_FORMAT, Level = DEFAULT_PRINT_LEVEL };
            
            VerifyAndCreateSourceDestination(config);
            VerifyAndCreatePrintDestination(config);
            VerifyAndCreateFormattersDestination(config);
        }

        private static void VerifyAndCreateFormattersDestination(DokimiConfig config)
        {
            if (config.Formatters == null)
            {
                config.Formatters = new FormattersInfo() { Formatters = new FormatterInfo[0] };
            }

            if (String.IsNullOrEmpty(config.Formatters.IncludePath))
            {
                config.Formatters.IncludePath = Path.Combine(GetAppCurrentDirectory(), DEFAULT_FORMATTERS_PATH);

                if (!Directory.Exists(config.Formatters.IncludePath))
                    Directory.CreateDirectory(config.Formatters.IncludePath);
            }
        }

        private static void VerifyAndCreateSourceDestination(DokimiConfig config)
        {
            if (config.Source == null)
                config.Source = new SourceInfo();

            if (String.IsNullOrEmpty(config.Source.IncludePath))
            {
                config.Source.IncludePath = Path.Combine(GetAppCurrentDirectory(), DEFAULT_SRC_PATH);

                if (!Directory.Exists(config.Source.IncludePath))
                    Directory.CreateDirectory(config.Source.IncludePath);
            }
        }

        private static void VerifyAndCreatePrintDestination(DokimiConfig config)
        {
            if (String.IsNullOrEmpty(config.Print.Destination))
            {
                config.Print.Destination = Path.Combine(GetAppCurrentDirectory(), DEFAULT_PRINT_DEST_PATH);

                if (!Directory.Exists(config.Print.Destination))
                    Directory.CreateDirectory(config.Print.Destination);
            }
        }

        private static string GetAppCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}