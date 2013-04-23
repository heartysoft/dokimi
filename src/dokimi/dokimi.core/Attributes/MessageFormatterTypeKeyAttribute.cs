using System;

namespace dokimi.core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MessageFormatterTypeKeyAttribute : Attribute
    {
        public string Type { get; private set; }
        public MessageFormattingVerbosity Verbosity { get; set; }

        public MessageFormatterTypeKeyAttribute(string type)
            : this(type, MessageFormattingVerbosity.Basic)
        {
        }

        public MessageFormatterTypeKeyAttribute(string type, MessageFormattingVerbosity verbosity)
        {
            Type = type;
            Verbosity = verbosity;
        }
    }

    public enum MessageFormattingVerbosity
    {
        Basic
    }
}