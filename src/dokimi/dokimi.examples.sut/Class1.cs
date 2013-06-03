using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using dokimi.core;
using dokimi.core.Specs;
using dokimi.nunit;

namespace dokimi.examples.sut
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    public abstract class SutTest : NUnitSpecificationTest
    {
    }

    public class FooTestCategory : SpecificationCategory
    {
        public FooTestCategory() : base("foo", "bar")
        {
        }
    }

    public class Foo : SutTest
    {
        public Specification Bar()
        {
            var spec =
                Specifications
                    .Catalog.Sut<Calculator, int, FooTestCategory>()
                    .Given("A calcultator", () => getCalculator())
                    .When(calc => doWork(calc))
                    .Then("foo", result => result == 5)
                    .Then(result => result == 5)
                    .Then(result => result == 5)
                    .Then(result => match(result))
                    .Build();

            return spec;
        }

        private static bool match(int result)
        {
            return result == 5;
        }

        private static int doWork(Calculator calc)
        {
            return calc.Add(2, 3);
        }

        private static Calculator getCalculator()
        {
            return new Calculator();
        }
    }

    public class Foo2 : SutTest
    {
        public Specification Bar()
        {
            var spec =
                Specifications
                    .Catalog.Sut<Calculator, int, FooTestCategory>()
                    .Given("A calculator", () => getCalculator())
                    .When("Numbers 2 and 3 are added", calc => addTwoNumbers(calc, 2, 3))
                    .Then(5);

            return spec;
        }

        private static bool match(int result)
        {
            return result == 5;
        }

        private static int addTwoNumbers(Calculator calc, int num1, int num2)
        {
            return calc.Add(num1, num2);
        }

        private static Calculator getCalculator()
        {
            return new Calculator();
        }
    }

    [Skip("Testing if skipping works.")]
    public class IgnoredSpec : SutTest
    {
        public Specification Bar()
        {
            var spec =
                Specifications.Catalog.Sut<Calculator, int, FooTestCategory>()
                             .Given("A calculator", () => new Calculator())
                             .When(x => x.Add(2, 3))
                             .Then(5);
            
            return spec;
        }
    }

    [Skip]
    public class IgnoredSpecWithEmptyReason : SutTest
    {
        public Specification Bar()
        {
            var spec =
                Specifications.Catalog.Sut<Calculator, int, FooTestCategory>()
                             .Given("A calculator", () => new Calculator())
                             .When(x => x.Add(2, 3))
                             .Then(5);

            return spec;
        }
    }
}
