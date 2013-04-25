using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace dokimi.core
{
    public class Expectations 
    {
        private readonly List<Expectation> _expectations = new List<Expectation>();

        public Expectations()
        {
        }

        public Expectations(IEnumerable<Expectation> expectations)
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
            foreach (var expectation in _expectations)
                expectation.DescribeTo(spec);
        }

        public void Verify<T>(T[] input, SpecInfo results, MessageFormatter formatter) 
        {
            var objects = input.Select(x => (object)x).ToArray();

            foreach (var expectation in _expectations)
                expectation.VerifyTo(objects, results);
        }

        public void AddExpectation(Expectation expectation)
        {
            _expectations.Add(expectation);
        }
    }
}