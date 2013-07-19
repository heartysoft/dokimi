using System;

namespace dokimi.Printers
{
    public class ConsoleSpecSuitePrinter : ISpecSuitePrinter
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