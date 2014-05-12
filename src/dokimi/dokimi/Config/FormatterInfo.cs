using System;
using System.Xml.Serialization;

namespace dokimi.Config
{
    [XmlType("formatter")]
    [Serializable]
    public class FormatterInfo
    {
        public string Type { get; set; }
        public string ContextName { get; set; }
        public string CategoryName { get; set; }
    }
}