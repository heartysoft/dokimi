using System;

namespace dokimi.core
{
    public interface Specification
    {
        SpecificationCategory SpecificationCategory { get; }
        void EnrichDescription(SpecInfo spec, MessageFormatter formatter);
        SpecInfo Run(SpecInfo spec, MessageFormatter formatter);
    }
}