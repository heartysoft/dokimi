using System;
using System.Collections;
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


        protected IEnumerable SpecificationFinder()
        {
            var specMethods =
                GetType().GetMethods().Where(x => typeof(Specification).IsAssignableFrom(x.ReturnType));

            foreach (var specMethod in specMethods)
            {
                var testCase = new TestCaseData(specMethod);
                testCase.SetName(getSpecName(specMethod.DeclaringType, specMethod));

                yield return testCase;
            }
        }

        [Test, TestCaseSource("SpecificationFinder")]
        public void Specification(MethodInfo testMethod)
        {
            var inconclusive = false;
            var testResult = new SpecInfo();

            var skip = testMethod.GetCustomAttributes().OfType<SkipAttributeContract>()
                    .FirstOrDefault();

            if (skip != null)
            {
                testResult.ReportSkipped(skip.Reason);
                inconclusive = true;
            }
            else
            {
                testResult.ReportSpecExecutionHasTriggered();
            }

            try
            {
                testResult.Name = getSpecName(testMethod.DeclaringType, testMethod);

                var spec = testMethod.Invoke(this, new object[0]) as Specification;
                var formatter = MessageFormatterRegistry.GetFormatter(spec.SpecificationCategory);

                testResult.UseFormatter(formatter);

                if (skip != null)
                    spec.EnrichDescription(testResult, formatter);
                else
                    testResult = spec.Run(testResult, formatter);
            }
            catch (Exception e)
            {
                testResult.Exception = e;
            }

            var sb = new StringBuilder();
            sb.AppendLine(getDescription(testResult));

            var description = sb.ToString();

            Console.WriteLine(description);

            if(inconclusive)
                Assert.Inconclusive(testResult.Skipped.Reason);
            else
                if(!testResult.Passed)
                    Assert.Fail(description);
                else
                    Assert.Pass(description);
        }

        private string getDescription(SpecInfo spec)
        {
            if (spec.Passed)
                return spec.ToString();

            var prefix = spec.Skipped.IsSkipped ? "  ???  " : "  ==>  ";
            
            var basicString = spec.ToString();
            var lines = basicString.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i != 0 && i != 1 && i != lines.Length - 1)
                    sb.Append(prefix);

                sb.AppendLine(lines[i]);
            }

            return sb.ToString();
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
