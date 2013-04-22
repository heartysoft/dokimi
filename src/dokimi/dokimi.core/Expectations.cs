using System;
using System.Linq;

namespace dokimi.core
{
    public class Expectations 
    {
        private readonly Expectation[] _expectations;

        private Expectations(Expectation[] expectations)
        {
            _expectations = expectations;
        }

        public static implicit operator Expectation[](Expectations step)
        {
            return step._expectations;
        }

        public static implicit operator Expectations(Expectation[] expectations)
        {
            return new Expectations(expectations);
        }

        public void DescribeTo(SpecInfo spec, MessageFormatter formatter)
        {
            foreach (string x in _expectations.Select(formatter.FormatMessage))
                spec.AddExpectationResult(x, false, null);
        }

        public void Verify<T>(T[] input, SpecInfo results, MessageFormatter formatter) where T:class 
        {
            foreach (var expectation in _expectations)
            {
                var description = formatter.FormatMessage(expectation);

                try
                {
                    expectation.Verify(input.Select(x =>(object)x).ToArray());
                    results.AddExpectationResult(description, true, null);
                }
                catch (Exception e)
                {
                    results.AddExpectationResult(description, false, e);
                }
            }
        }
    }
}