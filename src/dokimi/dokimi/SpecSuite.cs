using System;
using System.Collections.Generic;
using dokimi.core;

namespace dokimi
{
    [Serializable]
    public class SpecSuite
    {
        public SpecCategory[] Categories { get { return _categories.ToArray(); } }
        private readonly List<SpecCategory> _categories = new List<SpecCategory>();

        public SpecSuite Add(SpecCategory category)
        {
            _categories.Add(category);
            return this;
        }

        public void AddRange(SpecCategory[] categories)
        {
            foreach (var specCategory in categories)
                Add(specCategory);
        }
    }

    [Serializable]
    public class SpecCategory
    {
        public SpecificationCategory Name { get; private set; }
        public SpecInfo[] Specs { get { return _specs.ToArray(); } }

        private readonly List<SpecInfo> _specs = new List<SpecInfo>();

        public SpecCategory(SpecificationCategory name)
        {
            Name = name;
        }

        public SpecCategory Add(SpecInfo spec)
        {
            _specs.Add(spec);

            return this;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    public interface ISpecSuitePrinter
    {
        void Print(SpecSuite suite);
    }
}