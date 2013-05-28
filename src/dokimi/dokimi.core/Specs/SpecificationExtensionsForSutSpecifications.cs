// ReSharper disable CheckNamespace

using dokimi.core.Specs;

namespace dokimi.core
// ReSharper restore CheckNamespace
{
    public static class SpecificationExtensionsForSutSpecifications
    {
        public static SutSpecification<TSut, TResult>.ExpectingGiven Sut<TSut, TResult, TCategory>(this Specifcations specs)
            where TCategory : SpecificationCategory, new()
        {
            return Sut<TSut, TResult>(specs, new TCategory());
        }

        public static SutSpecification<TSut, TResult>.ExpectingGiven Sut<TSut, TResult>(this Specifcations specs,
                                                                                        string context, string category)
        {
            return Sut<TSut, TResult>(specs, new SpecificationCategory(context, category));
        }

        public static SutSpecification<TSut, TResult>.ExpectingGiven Sut<TSut, TResult>(this Specifcations specs, SpecificationCategory category)
        {
            return SutSpecification<TSut, TResult>.New(category);
        }
    }
}