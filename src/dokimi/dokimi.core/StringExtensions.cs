using System.Text;

namespace dokimi.core
{
    public static class StringExtensions
    {
        public static string ToWords(this string input)
        {
            var sb = new StringBuilder();
            bool started = false;
            var parts = input.ToCharArray();
            char last = 'a';

            foreach (var c in parts)
            {
                if (char.IsUpper(c))
                {
                    if (started)
                    {
                        if (last != ' ')
                            sb.Append(" ");
                        sb.Append(char.ToLowerInvariant(c));
                    }
                    else
                    {
                        started = true;
                        sb.Append(c);
                    }
                }
                else
                    sb.Append(c);

                last = c;
            }

            return sb.ToString();
        }
    }
}