using System;

namespace dokimi.core
{
    public interface Specification
    {
        SpecificationCategory Category { get; }
        void EnrichDescription(SpecInfo spec, MessageFormatter formatter);
        SpecInfo Run(SpecInfo results, MessageFormatter formatter);
    }

    public interface SpecificationStep
    {
        void DescribeTo(SpecInfo spec, MessageFormatter formatter);
    }

    public abstract class Specification<TGiven, TWhen, TWhenResult, TThen> : Specification 
        where TGiven : SpecificationStep
        where TWhen : SpecificationStep
        where TThen : VerificationInput
    {
        public SpecificationCategory Category { get; set; }
        public TGiven Given { get; set; }
        public TWhen When { get; set; }
        public Expectations Then { get; set; }
        public Func<TGiven, SpecInfo, MessageFormatter, Func<TWhen, TWhenResult>> OnGiven { get; set; }
        public Func<TWhenResult, TThen> OnWhen { get; set; }

        public void EnrichDescription(SpecInfo spec, MessageFormatter formatter)
        {
            Given.DescribeTo(spec, formatter);
            When.DescribeTo(spec, formatter);
            Then.DescribeTo(spec, formatter);
        }

        protected Specification()
        {
            if (Category == null || Category.IsUnspecified)
                Category = SpecificationCategory.Unspecified;
        }

        public SpecInfo Run(SpecInfo results, MessageFormatter formatter)
        {
            var onGiven = OnGiven(Given, results, formatter);
            When.DescribeTo(results, formatter);
            
            try
            {
                var whenResult = onGiven(When);
                results.When.Pass();
                var tthen = OnWhen(whenResult);
                tthen.Verify(Then, results, formatter);
            }
            catch
            {
                results.When.Fail();
                Then.DescribeTo(results, formatter);
                throw;
            }
            
            return results;
        }
        
    }
}