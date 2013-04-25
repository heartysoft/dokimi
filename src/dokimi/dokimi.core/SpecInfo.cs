using System;
using System.Collections.Generic;
using System.Linq;

namespace dokimi.core
{
    public class SpecInfo
    {
        public bool IsSkipped { get; private set; }
        public bool HasExecutionBeenTriggered { get; private set; }

        public string Name { get; set; }

        public bool Passed { get { return Exception == null && Thens.All(x => x.Passed); } }
        public Exception Exception;

        public StepInfo[] Givens { get { return _givens.ToArray(); } }
        public StepInfo When { get; private set; }
        public ExpectationInfo[] Thens { get { return _expectationResults.ToArray(); } }

        private readonly List<ExpectationInfo> _expectationResults = new List<ExpectationInfo>();

        public SpecInfo ReportExpectation(string description)
        {
            _expectationResults.Add(new ExpectationInfo(description));
            return this;
        }

        public SpecInfo ReportExpectationPass(string description)
        {
            _expectationResults.Add(new ExpectationInfo(description));
            return this;
        }

        public SpecInfo ReportExpectationFail(string description, Exception e)
        {
            _expectationResults.Add(new ExpectationInfo(description) { Exception = e, Passed = false});
            return this;
        }
        
        private readonly List<StepInfo> _givens = new List<StepInfo>();

        public SpecInfo ReportGivenStep(StepInfo given)
        {
            _givens.Add(given);
            return this;
        }

        public SpecInfo ReportWhenStep(StepInfo when)
        {
            When = when;
            return this;
        }

        public SpecInfo ReportSpecShouldBeSkipped()
        {
            IsSkipped = true;
            return this;
        }

        public SpecInfo ReportSpecExecutionHasTriggered()
        {
            HasExecutionBeenTriggered = true;
            return this;
        }
    }
}