using System;
using System.Collections.Generic;
using System.Linq;

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
    }

    public class SkipInfo
    {
        public string Reason { get; private set; }
        public bool IsSkipped { get { return !string.IsNullOrWhiteSpace(Reason); } }

        public const string Unspecified = "Unspecified";

        public SkipInfo(string reason)
        {
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