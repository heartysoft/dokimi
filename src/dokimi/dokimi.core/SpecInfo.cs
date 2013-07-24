using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dokimi.core
{
    public class SpecInfo
    {
        private MessageFormatter _formatter;
        public SkipInfo Skipped { get; private set; }
        public bool HasExecutionBeenTriggered { get; private set; }

        public string Name { get; set; }

        public bool Passed { get { return Exception == null && Thens.All(x => x.Passed); } }
        public Exception Exception;

        public StepInfo[] Givens { get { return _givens.ToArray(); } }
        public StepInfo When { get; private set; }
        public ExpectationInfo[] Thens { get { return _expectationResults.ToArray(); } }

        private readonly List<ExpectationInfo> _expectationResults = new List<ExpectationInfo>();

        public SpecInfo() : this(new DefaultFormatter())
        {
        }

        public SpecInfo(MessageFormatter formatter)
        {
            _formatter = formatter;
            Skipped = new SkipInfo();
        }
       
        private readonly List<StepInfo> _givens = new List<StepInfo>();

        public SpecInfo ReportGivenStep(StepInfo given)
        {
            _givens.Add(given);
            return this;
        }

        public SpecInfo ReportWhenStep(object when)
        {
            When = new StepInfo(formatMessage(when));
            return this;
        }

        public SpecInfo ReportSpecExecutionHasTriggered()
        {
            HasExecutionBeenTriggered = true;
            return this;
        }

        public void UseFormatter(MessageFormatter formatter)
        {
            _formatter = formatter;
        }

        public void ReportExpectation(Expectation expectation)
        {
            ReportExpectation(formatMessage(expectation));
        }

        public void ReportExpectation(string description)
        {
            var expectationInfo = new ExpectationInfo(description);
            _expectationResults.Add(expectationInfo);
        }

        public SpecInfo ReportExpectationPass(Expectation expectation)
        {
            return ReportExpectationPass(formatMessage(expectation));
        }

        public SpecInfo ReportExpectationPass(string description)
        {
            var expectationInfo = new ExpectationInfo(description);
            expectationInfo.Pass();
            _expectationResults.Add(expectationInfo);
            return this;
        }

        public SpecInfo ReportExpectationFail(Expectation expectation, Exception e)
        {
            return ReportExpectationFail(formatMessage(expectation), e);
        }

        public SpecInfo ReportExpectationFail(string description, Exception e)
        {
            var expectationInfo = new ExpectationInfo(description);
            expectationInfo.Fail(e);
            _expectationResults.Add(expectationInfo);
            return this;
        }

        private string formatMessage(object item)
        {
            return _formatter.FormatMessage(item);
        }

        public void ReportSkipped(string reason)
        {
            Skipped = new SkipInfo(reason);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            printName(sb, this);
            printSkipped(sb, this);
            printGiven(sb, this);
            printWhen(sb, this);
            printThen(sb, this);

            return sb.ToString();
        }

        private static void printThen(StringBuilder sb, SpecInfo spec)
        {
            sb.AppendLine("Then");

            foreach (var then in spec.Thens)
            {
                sb.Append(" - ");
                var description = spec.HasExecutionBeenTriggered
                                      ? string.Format("{0} [{1}]", then.Description, then.Passed ? "Passed" : "Failed")
                                      : then.Description;

                sb.AppendLine(description);


                if (!then.Passed && then.Exception != null)
                {
                    sb.AppendLine("_____________________________________________");
                    sb.AppendLine(then.Exception.ToString());
                    sb.AppendLine("_____________________________________________");
                    sb.AppendLine();
                }
            }
        }

        private static void printWhen(StringBuilder sb, SpecInfo spec)
        {
            sb.AppendLine("When");

            sb.Append(" - ");
            sb.AppendLine(spec.HasExecutionBeenTriggered
                ? string.Format("{0} [{1}]", spec.When.Description, spec.When.Passed ? "Passed" : "Failed")
                : spec.When.Description);

            if (shouldDisplayWhenException(spec))
            {
                sb.AppendLine(spec.Exception.ToString());
                sb.AppendLine(spec.Exception.StackTrace);
            }
        }

        private static bool shouldDisplayWhenException(SpecInfo spec)
        {
            return !spec.When.Passed && spec.Exception != null
                && spec.Givens.All(x => x.Passed);
        }

        private static void printGiven(StringBuilder sb, SpecInfo spec)
        {
            if (spec.Givens.Length == 0)
                return;

            sb.AppendLine("Given");

            foreach (var given in spec.Givens)
            {
                var description = spec.HasExecutionBeenTriggered
                                      ? string.Format("{0} [{1}]", given.Description,
                                                      given.Passed ? "Passed" : "Failed")
                                      : given.Description;

                sb.Append(" - ");
                sb.AppendLine(description);
            }


            if (!spec.Givens.All(x => x.Passed) && spec.Exception != null)
            {
                sb.AppendLine(spec.Exception.ToString());
            }
        }

        private static void printSkipped(StringBuilder sb, SpecInfo specInfo)
        {
            if (specInfo.Skipped.IsSkipped)
            {
                sb.AppendLine(specInfo.Skipped.ToString());
            }
        }

        private static void printName(StringBuilder sb, SpecInfo specInfo)
        {
            sb.AppendLine(specInfo.Name);
            sb.AppendLine("=============================================================");
        }
    }

    public class SkipInfo
    {
        public string Reason { get; private set; }
        public bool IsSkipped { get { return !string.IsNullOrWhiteSpace(Reason); } }

        private const string Unspecified = "Unspecified";

        public SkipInfo(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                reason = Unspecified;
            Reason = reason;
        }

        public SkipInfo()
        {
        }

        public override string ToString()
        {
            return string.Format("[Skipped. Reason: {0}]", Reason);
        }
    }
}