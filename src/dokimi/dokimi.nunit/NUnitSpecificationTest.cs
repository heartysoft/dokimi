using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var testResult = new SpecInfo();

            try
            {
                var specMethods =
                    GetType().GetMethods().Where(x => typeof(Specification).IsAssignableFrom(x.ReturnType));

                var testMethod = specMethods.Single();

                var spec = testMethod.Invoke(this, new object[0]) as Specification;
                var formatter = MessageFormatterRegistry.GetFormatter(spec.SpecificationCategory);

                testResult.UseFormatter(formatter);
                testResult = spec.Run(testResult, formatter);
            }
            catch (Exception e)
            {
                testResult.Exception = e;
            }

            if (testResult.Passed)
                return;

            assertFailure(testResult);
        }

        private static void assertFailure(SpecInfo specInfo)
        {
            var sb = new StringBuilder();

            if (specInfo.Exception != null)
                sb.AppendLine(specInfo.Exception.ToString());
            else
            {
                var failures = specInfo.Thens.Where(x => !x.Passed);

                
                foreach (var failure in failures)
                {
                    sb.AppendLine(string.Format("Failed specification: {0}", failure.Description));
                    getGherkin(specInfo, sb);
                    sb.AppendLine();
                    sb.AppendLine(string.Format("Exception: {0}", failure.Exception));
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            Assert.Fail(sb.ToString());
        }

        private static void getGherkin(SpecInfo specInfo, StringBuilder sb)
        {
            var failedGivens = specInfo.Givens.Where(x => x.Passed).ToList();
            var failedThens = specInfo.Thens.Where(x => !x.Passed).ToList();
            var when = specInfo.When.Description;

            foreach (var given in failedGivens)
                sb.AppendLine(string.Format("Given {0}", given.Description));

            sb.AppendLine(string.Format("When {0}", when));

            foreach (var then in failedThens)
                sb.AppendLine(string.Format("Then {0}", then.Description));
        }
    }
}
