using System;

namespace dokimi.core
{
    public class ExpectationInfo : StepInfo
    {
        public Exception Exception { get; private set; }

        public ExpectationInfo(string description) : base(description)
        {
        }

        public ExpectationInfo Fail(Exception e)
        {
            Exception = e;
            Fail();
            return this;
        }
    }
}