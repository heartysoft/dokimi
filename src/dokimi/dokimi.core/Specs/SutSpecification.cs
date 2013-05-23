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

        public SpecificationCategory SpecificationCategory { get; private set; }
        
        public void EnrichDescription(SpecInfo spec, MessageFormatter formatter)
        {
            _given.DescribeTo(spec, formatter);
            _when.DescribeTo(spec);
            _expectations.DescribeTo(spec, formatter);
        }

        public SpecInfo Run(SpecInfo spec, MessageFormatter formatter)
        {
            try
            {
                _given.DescribeTo(spec, formatter);
                _when.DescribeTo(spec);

                var sut = _given.GetSut(spec);
                spec.Givens[0].Pass();

                var result = _when.GetResult(sut);
                spec.When.Pass();

                _expectations.Verify(new object[] { result }, spec, formatter);
            }
            catch
            {
                _expectations.DescribeTo(spec, formatter);
                throw;
            }
            
            return spec;
        }

        public SutSpecification<TSut, TResult> Category<T>() where T : SpecificationCategory, new()
        {
            SpecificationCategory = new T();
            return this;
        }

        public SutSpecification<TSut, TResult> Category<T>(T category) where T : SpecificationCategory
        {
            SpecificationCategory = category;
            return this;
        }

        public SutSpecification<TSut, TResult> Category(string contextName, string categoryName)
        {
            SpecificationCategory = new SpecificationCategory(contextName, categoryName);
            return this;
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

    public class GivenSut<TSut> 
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
            
            spec.ReportGivenStep(new StepInfo(description));
        }

        public TSut GetSut(SpecInfo info)
        {
            var result = _given.Compile()();
            return result;
        }
    }

    public class WhenSut<TSut, TResult> 
    {
        private readonly Expression<Func<TSut, TResult>> _when;
        private readonly string _description;
        
        public WhenSut(Expression<Func<TSut, TResult>> when, string description)
        {
            _when = when;
            _description = description;
        }

        public void DescribeTo(SpecInfo spec)
        {
            if (!string.IsNullOrWhiteSpace(_description))
                spec.ReportWhenStep(_description);
            else
                spec.ReportWhenStep(_when);
        }

        public TResult GetResult(TSut sut)
        {
            var result = _when.Compile()(sut);
            return result;
        }
    }
}