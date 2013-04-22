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
                var formatter = MessageFormatterRegistry.GetFormatter(spec.Category);
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
                    sb.AppendLine(failure.Description);
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            Assert.Fail(sb.ToString());
        }
    }
}
