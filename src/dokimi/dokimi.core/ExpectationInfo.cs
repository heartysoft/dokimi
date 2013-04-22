using System;

namespace dokimi.core
{
    public class ExpectationInfo : StepInfo
    {
        public Exception Exception;

        public ExpectationInfo(string description) : base(description)
        {
        }
    }
}