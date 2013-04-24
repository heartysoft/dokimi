using System;
using System.Linq;
using System.Linq.Expressions;

namespace dokimi.core.Specs
{
    public class SutSpecification<TSut, TResult> : Specification<GivenSut<TSut>, WhenSut<TSut, TResult>, TResult, SutVerificationInfo<TResult>>
    {
        public SutSpecification()
        {
            base.Then = new Expectation[0];

            OnGiven = (given, spec, formatter) =>
                          {
                              given.DescribeTo(spec, formatter);
                              var sut = given.GetSut();
                              spec.Givens.Select(x => x.Pass()).ToArray();
                              return when => when.GetResult(sut, spec, formatter);
                          };

            OnWhen = result => new SutVerificationInfo<TResult>(result);
        }

        public new SutSpecification<TSut, TResult> When(string description, Expression<Func<TSut, TResult>> executor)
        {
            base.When = new WhenSut<TSut, TResult>(executor, description);
            return this;
        }

        public new SutSpecification<TSut, TResult> When(Expression<Func<TSut, TResult>> executor)
        {
            base.When = new WhenSut<TSut, TResult>(executor, string.Empty);
            return this;
        }


        public new SutSpecification<TSut, TResult> Given(Expression<Func<TSut>> factory)
        {
            return Given(string.Empty, factory);
        }

        public new SutSpecification<TSut, TResult> Given(string description, Expression<Func<TSut>> factory)
        {
            base.Given = new GivenSut<TSut>(factory, description);
            return this;
        }

        public new SutSpecification<TSut, TResult> Then(Expression<Func<TResult, bool>> expectation)
        {
            return Then(string.Empty, expectation);
        }

        public new SutSpecification<TSut, TResult> Then(string description, Expression<Func<TResult, bool>> expectation)
        {
            var exp = new Expectation<TResult>(expectation, description);
            base.Then.AddExpectation(exp);

            return this;
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

        public TSut GetSut()
        {
            return _given.Compile()();
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
            var result =  _when.Compile()(sut);

            spec.When.Pass();
            return result;
        }
    }

    public class SutVerificationInfo<T> : VerificationInput
    {
        private readonly T _result;

        public SutVerificationInfo(T result)
        {
            _result = result;
        }

        public void Verify(Expectations expectations, SpecInfo results, MessageFormatter formatter)
        {
            expectations.Verify(new []{_result}, results, formatter);
        }
    }
}