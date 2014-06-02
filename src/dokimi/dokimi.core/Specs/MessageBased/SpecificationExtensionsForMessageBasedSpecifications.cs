using System.Globalization;
using dokimi.core.Specs.MessageBased;
// ReSharper disable CheckNamespace
using dokimi.core.Specs;

namespace dokimi.core
// ReSharper restore CheckNamespace
{

    namespace dokimi.core.Specs.MessageBased
    {
        public static class SpecificationExtensionsForMessageBasedSpecifications
        {
            public static MessageBasedSpecification.ExpectingEvent MessageBasedSpecification<T>(this Specifications spec)
                where T : SpecificationCategory, new()
            {
                return global::dokimi.core.Specs.MessageBased.MessageBasedSpecification.New(new T());
            }

            public static MessageBasedSpecification.ExpectingEvent MessageBasedSpecification(this Specifications spec,
                string context, string category)
            {
                return
                    global::dokimi.core.Specs.MessageBased.MessageBasedSpecification.New(
                        new SpecificationCategory(context, category));
            }

            public static MessageBasedSpecification.ExpectingEvent MessageBasedSpecification(this Specifications spec,
                SpecificationCategory category)
            {
                return global::dokimi.core.Specs.MessageBased.MessageBasedSpecification.New(category);
            }
        }
    }
}