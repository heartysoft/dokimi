using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace dokimi.core
{
    public class Expectations 
    {
        private readonly List<Expectation> _expectations = new List<Expectation>();

        private Expectations(Expectation[] expectations)
        {
            _expectations.AddRange(expectations);
        }

        public static implicit operator Expectation[](Expectations step)
        {
            return step._expectations.ToArray();
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

        public void Verify<T>(T[] input, SpecInfo results, MessageFormatter formatter) 
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

        public void AddExpectation(Expectation expectation)
        {
            _expectations.Add(expectation);
        }
    }
}