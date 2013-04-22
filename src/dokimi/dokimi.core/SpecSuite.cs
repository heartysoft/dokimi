using System;
using System.Collections.Generic;

namespace dokimi.core
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


    public class SpecSuiteConsolePrinter : ISpecSuitePrinter
    {
        private void print(SpecCategory specCategory)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(specCategory.Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("================================================================================");

            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var spec in specCategory.Specs)
            {
                Console.WriteLine(spec.Name);
                Console.WriteLine();

                printHeading("Given");

                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (var given in spec.Givens)
                    Console.WriteLine("\t{0}", given.Description);

                printHeading("When");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\t{0}", spec.When.Description);

                printHeading("Then");

                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (var then in spec.Thens)
                    Console.WriteLine("\t{0}", then.Description);

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static void printHeading(string heading)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(heading);
        }

        public void Print(SpecSuite suite)
        {
            foreach (var category in suite.Categories)
            {
                print(category);
            }
        }
    }
}