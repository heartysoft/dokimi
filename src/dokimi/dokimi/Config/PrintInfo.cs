using System;

namespace dokimi.Config
{
    [Serializable]
    public class PrintInfo
    {
        public PrintLevelInfo Level { get; set; }
        public PrintFormatInfo Format { get; set; }
        public string Destination { get; set; }
    }
}