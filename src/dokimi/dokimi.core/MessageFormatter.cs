using System;
using System.Collections.Generic;

namespace dokimi.core
{
    public interface MessageFormatter
    {
        string FormatMessage(object message);
    }

    public static class MessageFormatterRegistry
    {
        static readonly Dictionary<SpecificationCategory, MessageFormatter> _formatters = new Dictionary<SpecificationCategory, MessageFormatter>();
        public static MessageFormatter GetFormatter(SpecificationCategory category)
        {
            if (_formatters.ContainsKey(category))
                return _formatters[category];

            return new DefaultFormatter();
        }

        public static void AddFormatter(SpecificationCategory category, MessageFormatter formatter)
        {
            _formatters[category] = formatter;
        }
    }

    public class DefaultFormatter : MessageFormatter
    {
        readonly Dictionary<Type, Func<object, string>> _mappings = new Dictionary<Type, Func<object, string>>();

        public string FormatMessage(object message)
        {
            var type = message.GetType();
            if (_mappings.ContainsKey(type))
                return _mappings[type](message);

            return message.ToString();
        }

        protected void Register<T>(Func<T, string> formatter)
        {
            Func<object, string> widenedFormatter = x => formatter((T)x);
            _mappings[typeof(T)] = widenedFormatter;
        }
    }
}