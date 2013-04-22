using System;
using System.Text;

namespace dokimi.core
{
    public class ExpectationFailedException<T> : Exception
    {
        public ExpectationFailedException(Expectation<T> expectation, object[] generatedEvents)
            : base(generateMessage(expectation, generatedEvents))
        {
        }

        private static string generateMessage(Expectation<T> expectation, object[] generatedEvents)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Expected {0}", expectation);
            sb.AppendLine();
            sb.AppendFormat("but the following {0} messages were generated: ", generatedEvents.Length);
            sb.AppendLine();

            foreach (var e in generatedEvents)
            {
                sb.AppendLine(e.ToString());
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return Message;
        }
    }
}