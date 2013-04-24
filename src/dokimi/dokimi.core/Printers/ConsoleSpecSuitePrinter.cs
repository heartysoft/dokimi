using System;

namespace dokimi.core.Printers
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