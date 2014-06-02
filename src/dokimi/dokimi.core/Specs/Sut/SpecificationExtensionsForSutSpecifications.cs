using dokimi.core.Specs.Sut;
// ReSharper disable CheckNamespace
using dokimi.core.Specs;

namespace dokimi.core
// ReSharper restore CheckNamespace
{
    public static class SpecificationExtensionsForSutSpecifications
    {
        public static SutSpecification<TSut, TResult>.ExpectingGiven Sut<TSut, TResult, TCategory>(this Specifications specs)
            where TCategory : SpecificationCategory, new()
        {
            return Sut<TSut, TResult>(specs, new TCategory());
        }

        public static SutSpecification<TSut, TResult>.ExpectingGiven Sut<TSut, TResult>(this Specifications specs,
                                                                                        string context, string category)
        {
            return Sut<TSut, TResult>(specs, new SpecificationCategory(context, category));
        }

        public static SutSpecification<TSut, TResult>.ExpectingGiven Sut<TSut, TResult>(this Specifications specs, SpecificationCategory category)
        {
            return SutSpecification<TSut, TResult>.New(category);
        }
    }
}