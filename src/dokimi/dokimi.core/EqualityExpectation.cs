using System.Linq;
using KellermanSoftware.CompareNetObjects;

namespace dokimi.core
{
    public class EqualityExpectation : Expectation
    {
        private readonly object _expected;
        private readonly string _description;
        readonly CompareLogic _comparer = new CompareLogic();

        public EqualityExpectation(object expected)
            :this(expected, null)
        {
        }

        public EqualityExpectation(object expected, string description)
        {
            _expected = expected;
            _description = description;
        }

        public void DescribeTo(SpecInfo spec, MessageFormatter formatter)
        {
            spec.ReportExpectation(formatter.FormatMessage(_expected));
        }

        public void VerifyTo(object[] input, SpecInfo results, MessageFormatter formatter)
        {
            if (input.Any(x => _comparer.Compare(x, _expected).AreEqual))
            {
                results.ReportExpectationPass(this);
                return;
            }

            results.ReportExpectationFail(this, new ExpectationFailedException(this, input));
        }

        public override string ToString()
        {
            return _description ?? _expected.ToString();
        }
    }
}