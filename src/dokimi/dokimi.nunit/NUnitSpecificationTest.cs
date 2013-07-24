using System;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using dokimi.core;

namespace dokimi.nunit
{
    [TestFixture]
    public abstract class NUnitSpecificationTest : SpecificationTest
    {
        public Func<Specification, Specification> TransformForEnvironment = x => x;

        [Test]
        public void Execute()
        {
            var allSpecTestsPassed = true;
            var testResultBuilder = new StringBuilder();

            var specMethods =
                GetType().GetMethods().Where(x => typeof(Specification).IsAssignableFrom(x.ReturnType));
            foreach (var testMethod in specMethods)
            {
                var testResult = new SpecInfo();
                testResult.ReportSpecExecutionHasTriggered();

                try
                {
                    testResult.Name = getSpecName(testMethod.DeclaringType, testMethod);
                    testResult.Name = getSpecName(testMethod.DeclaringType, testMethod);

                    var spec = testMethod.Invoke(this, new object[0]) as Specification;
                    var formatter = MessageFormatterRegistry.GetFormatter(spec.SpecificationCategory);

                    testResult.UseFormatter(formatter);
                    testResult = spec.Run(testResult, formatter);
                }
                catch (Exception e)
                {
                    testResult.Exception = e;
                }

                testResultBuilder.AppendLine(testResult.ToString());

                if (!testResult.Passed && allSpecTestsPassed)
                    allSpecTestsPassed = false;
            }

            if (allSpecTestsPassed)
            {
                Console.WriteLine(testResultBuilder);
                return;
            }

            Assert.Fail(testResultBuilder.ToString());
        }

        private static string getSpecName(Type type, MethodInfo methodInfo)
        {
            var raw = type.Name + " " + methodInfo.Name;
            var underscoreReplaced = raw.Replace('_', ' ');

            var charArray = underscoreReplaced.ToCharArray();
            charArray[0] = char.ToUpperInvariant(charArray[0]);
            var firstLetterCapitalised = new string(charArray);

            return firstLetterCapitalised;
        }
    }
}
