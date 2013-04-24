using System;
using System.Collections.Generic;
using System.Linq;
using dokimi.Config;

namespace dokimi
{
    [Serializable]
    public class RunSpecCommand
    {
        public string AssemblyPath { get; private set; }
        public string FormattersPath { get; private set; }
        public FormatterInfo[] Formatters { get; private set; }
        public PrintInfo PrintInfo { get; private set; }
        public ActionInfo Action { get; private set; }

        public RunSpecCommand(string assemblyPath, string formattersPath, IEnumerable<FormatterInfo> formatters, PrintInfo printInfo, ActionInfo action)
        {
            AssemblyPath = assemblyPath;
            FormattersPath = formattersPath;
            PrintInfo = printInfo;
            Action = action;
            Formatters = formatters.ToArray();
        }
    }
}