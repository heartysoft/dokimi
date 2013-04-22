namespace dokimi.core
{
    public interface VerificationInput
    {
        void Verify(Expectations expectations, SpecInfo results, MessageFormatter formatter);
    }

    public class NewMessagesVerificationInput : VerificationInput
    {
        private readonly object[] _newObjects;

        private NewMessagesVerificationInput(object[] newObjects)
        {
            _newObjects = newObjects;
        }

        public static implicit operator object[](NewMessagesVerificationInput input)
        {
            return input._newObjects;
        }

        public static implicit operator NewMessagesVerificationInput(object[] objects)
        {
            return new NewMessagesVerificationInput(objects);
        }

        public void Verify(Expectations expectations, SpecInfo results, MessageFormatter formatter)
        {
            expectations.Verify(_newObjects, results, formatter);
        }
    }
}