using System.Xml.Serialization;

namespace dokimi.Config
{
    public class DokimiConfig
    {
        public ActionInfo Action { get; set; }
        public SourceInfo Source { get; set; }
        public PrintInfo Print { get; set; }
        public FormattersInfo Formatters { get; set; }
    }
}