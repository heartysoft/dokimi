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

    public class Foo : SutTest
    {
        public Specification Bar()
        {
            var spec = new SutSpecification<Calculator, int>();
            spec.Category = new SpecificationCategory("foo", "bar");

            spec.Given("A calculator", () => getCalculator());
            spec.When(calc => doWork(calc));
            spec.Then("foo", result => result == 5);
            spec.Then(result => result == 5);
            spec.Then(result => result == 5);
            spec.Then(result => match(result));

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

    [Skip("Testing if skipping works.")]
    public class IgnoredSpec : SutTest
    {
        public Specification Bar()
        {
            var spec = new SutSpecification<Calculator, int>();
            spec.Category = new SpecificationCategory("foo", "bar");

            spec.Given("A calculator", () => new Calculator());
            spec.When(calc => calc.Add(2, 3));
            spec.Then("foo", result => result == 5);

            return spec;
        }
    }

    [Skip]
    public class IgnoredSpecWithEmptyReason : SutTest
    {
        public Specification Bar()
        {
            var spec = new SutSpecification<Calculator, int>();
            spec.Category = new SpecificationCategory("foo", "bar");

            spec.Given("A calculator", () => new Calculator());
            spec.When(calc => calc.Add(2, 3));
            spec.Then("foo", result => result == 5);

            return spec;
        }
    }
}
