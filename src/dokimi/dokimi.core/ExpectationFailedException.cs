using System;
using System.Text;

namespace dokimi.core
{
    public class ExpectationFailedException : Exception
    {
        public ExpectationFailedException(Expectation expectation, object[] generatedEvents)
            : base(generateMessage(expectation, generatedEvents))
        {
        }

        private static string generateMessage(Expectation expectation, object[] generatedEvents)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Expected {0}", expectation);
            sb.AppendLine();
            sb.AppendFormat("but the following {0} result(s) were found: ", generatedEvents.Length);
            sb.AppendLine();
            
            sb.Append(string.Join(Environment.NewLine, generatedEvents));

            return sb.ToString();
        }

        public override string ToString()
        {
            return Message;
        }
    }
}