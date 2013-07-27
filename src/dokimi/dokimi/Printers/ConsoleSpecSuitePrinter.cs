using System;

namespace dokimi.Printers
{
    public class ConsoleSpecSuitePrinter : ISpecSuitePrinter
    {
        private void print(SpecCategory specCategory)
        {
            Console.WriteLine(specCategory.Name);
            Console.WriteLine("================================================================================");

            foreach (var spec in specCategory.Specs)
            {
                Console.WriteLine(spec.ToString());
            }
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