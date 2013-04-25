using System;
using System.Linq;
using System.Linq.Expressions;

namespace dokimi.core.Specs
{
    public class SutSpecification<TSut, TResult> : Specification
    {
        private GivenSut<TSut> _given;
        private WhenSut<TSut, TResult> _when;
        private readonly Expectations _expectations = new Expectations();

        public SpecificationCategory Category { get; set; }
        
        public void EnrichDescription(SpecInfo spec, MessageFormatter formatter)
        {
            _given.DescribeTo(spec, formatter);
            _when.DescribeTo(spec, formatter);
            _expectations.DescribeTo(spec, formatter);
        }

        public SpecInfo Run(SpecInfo results, MessageFormatter formatter)
        {
            var sut = _given.GetSut(results, formatter);
            var result = _when.GetResult(sut, results, formatter);
            _expectations.Verify(new object[]{result}, results, formatter);

            return results;
        }

        public SutSpecification<TSut, TResult> Given(string description, Expression<Func<TSut>> given)
        {
            _given = new GivenSut<TSut>(given, description);
            return this;
        }

        public SutSpecification<TSut, TResult> When(string description, Expression<Func<TSut, TResult>> when)
        {
            _when = new WhenSut<TSut, TResult>(when, description);
            return this;
        }

        public SutSpecification<TSut, TResult> Given(Expression<Func<TSut>> given)
        {
            return Given(string.Empty, given);
        }

        public SutSpecification<TSut, TResult> When(Expression<Func<TSut, TResult>> when)
        {
            return When(string.Empty, when);
        }

        public SutSpecification<TSut, TResult> Then(string description, Expression<Func<TResult, bool>> expectation)
        {
            _expectations.AddExpectation(new Expectation<TResult>(expectation, description));
            return this;
        }

        public SutSpecification<TSut, TResult> Then(Expression<Func<TResult, bool>> expectation)
        {
            return Then(string.Empty, expectation);
        }
    }

    public class GivenSut<TSut> : SpecificationStep
    {
        private readonly Expression<Func<TSut>> _given;
        private readonly string _description;

        public GivenSut(Expression<Func<TSut>> given, string description)
        {
            _given = given;
            _description = description;
        }

        public void DescribeTo(SpecInfo spec, MessageFormatter formatter)
        {
            var description = string.IsNullOrWhiteSpace(_description) ? formatter.FormatMessage(_given) : _description;
            
            spec.Givens = new[]{new StepInfo(description)};
        }

        public TSut GetSut(SpecInfo info, MessageFormatter formatter)
        {
            DescribeTo(info, formatter);
            var result = _given.Compile()();
            info.Givens.Select(x => x.Pass()).ToArray();

            return result;
        }
    }

    public class WhenSut<TSut, TResult> : SpecificationStep
    {
        private readonly Expression<Func<TSut, TResult>> _when;
        private readonly string _description;
        
        public WhenSut(Expression<Func<TSut, TResult>> when, string description)
        {
            _when = when;
            _description = description;
        }

        public void DescribeTo(SpecInfo spec, MessageFormatter formatter)
        {
            var description = string.IsNullOrWhiteSpace(_description) ? formatter.FormatMessage(_when) : _description;

            spec.When = new StepInfo(description);
        }

        public TResult GetResult(TSut sut, SpecInfo spec, MessageFormatter formatter)
        {
            DescribeTo(spec, formatter);
            var result = _when.Compile()(sut);

            spec.When.Pass();
            return result;
        }
    }
}