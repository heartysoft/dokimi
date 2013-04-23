namespace dokimi.core
{
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
}