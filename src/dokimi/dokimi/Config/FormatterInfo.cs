using System.Xml.Serialization;

namespace dokimi.Config
{
    [XmlType("formatter")]
    public class FormatterInfo
    {
        public string Type { get; set; }
        public string ContextName { get; set; }
        public string CategoryName { get; set; }
    }
}