using System;
using System.Collections.Generic;
using System.Linq;

namespace dokimi.core
{
    public class SpecInfo
    {
        public bool IsIgnored;
        public bool HasExecuted;

        public string Name { get; set; }

        public bool Passed { get { return Exception == null && Thens.All(x => x.Passed); } }
        public Exception Exception;

        public StepInfo[] Givens { get; set; }
        public StepInfo When { get; set; }
        public ExpectationInfo[] Thens { get { return _expectationResults.ToArray(); } }

        private readonly List<ExpectationInfo> _expectationResults = new List<ExpectationInfo>();

        public SpecInfo AddExpectationResult(string description, bool passed, Exception exception)
        {
            _expectationResults.Add(new ExpectationInfo(description)
            {
                Passed = passed,
                Exception = exception
            });
            return this;
        }
    }

    public class StepInfo
    {
        public string Description { get; private set; }
        public bool Passed { get; set; }

        public StepInfo(string description)
        {
            Description = description;
        }

        public StepInfo Pass()
        {
            Passed = true;
            return this;
        }

        public StepInfo Fail()
        {
            Passed = false;
            return this;
        }
    }

    public class ExpectationInfo : StepInfo
    {
        public Exception Exception;

        public ExpectationInfo(string description) : base(description)
        {
        }
    }
}