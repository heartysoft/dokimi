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

        internal static ExpectingGiven New(SpecificationCategory category)
        {
            var instance = new SutSpecification<TSut, TResult>(category);
            return new ExpectingGiven(instance);
        }

        private SutSpecification(SpecificationCategory specificationCategory)
        {
            SpecificationCategory = specificationCategory;
        }

        public class ExpectingGiven
        {
            private readonly SutSpecification<TSut, TResult> _instance;

            public ExpectingGiven(SutSpecification<TSut, TResult> instance)
            {
                _instance = instance;
            }

            public ExpectingWhen Given(Expression<Func<TSut>> factory)
            {
                return Given(null, factory);
            }

            public ExpectingWhen Given(string description, Expression<Func<TSut>> factory)
            {
                _instance._given = new GivenSut<TSut>(factory, description);
                return new ExpectingWhen(_instance);
            }
        }

        public class ExpectingWhen
        {
            private readonly SutSpecification<TSut, TResult> _instance;

            public ExpectingWhen(SutSpecification<TSut, TResult> instance)
            {
                _instance = instance;
            }

            public ExpectingThen When(Expression<Func<TSut, TResult>> when)
            {
                return When(null, when);
            }

            public ExpectingThen When(string description, Expression<Func<TSut, TResult>> when)
            {
                _instance._when = new WhenSut<TSut, TResult>(when, description);
                return new ExpectingThen(_instance);
            }
        }

        public class ExpectingThen
        {
            private readonly SutSpecification<TSut, TResult> _instance;

            public ExpectingThen(SutSpecification<TSut, TResult> instance)
            {
                _instance = instance;
            }

            public ExpectingAnotherThen Then(Expression<Func<TResult, bool>> expectation)
            {
                return Then(null, expectation);
            }

            public ExpectingAnotherThen Then(string description, Expression<Func<TResult, bool>> expectation)
            {
                _instance._expectations.AddExpectation(new Expectation<TResult>(expectation, description));
                return new ExpectingAnotherThen(_instance);
            }

            public SutSpecification<TSut, TResult> Then(TResult expected)
            {
                return Then(null, expected);
            }

            public SutSpecification<TSut, TResult> Then(string description, TResult expected)
            {
                _instance._expectations.AddExpectation(new EqualityExpectation(expected, description));
                return _instance;
            }
        }

        public class ExpectingAnotherThen
        {
            private readonly SutSpecification<TSut, TResult> _instance;

            public ExpectingAnotherThen(SutSpecification<TSut, TResult> instance)
            {
                _instance = instance;
            }

            public ExpectingAnotherThen Then(Expression<Func<TResult, bool>> expectation)
            {
                return Then(null, expectation);
            }

            public ExpectingAnotherThen Then(string description, Expression<Func<TResult, bool>> expectation)
            {
                _instance._expectations.AddExpectation(new Expectation<TResult>(expectation, description));
                return new ExpectingAnotherThen(_instance);
            }

            public SutSpecification<TSut, TResult> Build()
            {
                return _instance;
            }
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