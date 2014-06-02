using System;
using System.Linq.Expressions;

namespace dokimi.core.Specs.MessageBased
{
    public partial class MessageBasedSpecification
    {
        public class ExpectingSinkWireup
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingSinkWireup(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public ExpectingEvent WireupSink(Action<Action<object>> wireup)
            {
                _instance._sinkWireup = wireup;
                return new ExpectingEvent(_instance);
            }
        }

        public class ExpectingEvent
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingEvent(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public ExpectingAnotherEventOrWhen Given(string description, object @event)
            {
                _instance._given.Add(new MessageEntry(description, @event));
                return new ExpectingAnotherEventOrWhen(_instance);
            }

            public ExpectingAnotherEventOrWhen Given(object @event)
            {
                return Given(null, @event);
            }


            public ExpectingWhen NoHistory()
            {
                return new ExpectingWhen(_instance);
            }
            
        }

        public class ExpectingAnotherEventOrWhen
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingAnotherEventOrWhen(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public ExpectingAnotherEventOrWhen And(string description, object @event)
            {
                _instance._given.Add(new MessageEntry(description, @event));
                return this;
            }

            public ExpectingAnotherEventOrWhen And(object @event)
            {
                return And(null, @event);
            }

            public ExpectingThen When(string description, object message)
            {
                return new ExpectingWhen(_instance).When(description, message);
            }

            public ExpectingThen When(object message)
            {
                return new ExpectingWhen(_instance).When(message);
            }

        }

        public class ExpectingWhen
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingWhen(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public ExpectingThen When(string description, object message)
            {
                _instance._when = new MessageEntry(description, message);
                return new ExpectingThen(_instance);
            }

            public ExpectingThen When(object message)
            {
                _instance._when = new MessageEntry(null, message);
                return new ExpectingThen(_instance);
            }
        }

        public class ExpectingThen
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingThen(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public ExpectingAnotherThenOrWireup Then(string description, object @event)
            {
                _instance._expectations.AddExpectation(new EqualityExpectation(@event, description));
                return new ExpectingAnotherThenOrWireup(_instance);
            }

            public ExpectingAnotherThenOrWireup Then(object @event)
            {
                _instance._expectations.AddExpectation(new EqualityExpectation(@event));
                return new ExpectingAnotherThenOrWireup(_instance);
            }

            public ExpectingAnotherThenOrWireup Then<T>(string description, Expression<Func<T, bool>> expectation)
            {
                _instance._expectations.AddExpectation(new Expectation<T>(expectation, description));
                return new ExpectingAnotherThenOrWireup(_instance);
            }

            public ExpectingAnotherThenOrWireup Then<T>(Expression<Func<T, bool>> expectation)
            {
                _instance._expectations.AddExpectation(new Expectation<T>(expectation));
                return new ExpectingAnotherThenOrWireup(_instance);
            }

            public ExpectingWireup NothingShouldHappen()
            {
                _instance._expectations.AddExpectation(new MessageCount(0));
                return new ExpectingWireup(_instance);
            }
        }

        public class ExpectingAnotherThenOrWireup
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingAnotherThenOrWireup(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public ExpectingAnotherThenOrWireup And(string description, object @event)
            {
                _instance._expectations.AddExpectation(new EqualityExpectation(@event, description));
                return this;
            }

            public ExpectingAnotherThenOrWireup And(object @event)
            {
                _instance._expectations.AddExpectation(new EqualityExpectation(@event));
                return this;
            }

            public ExpectingAnotherThenOrWireup And<T>(string description, Expression<Func<T, bool>> expectation)
            {
                _instance._expectations.AddExpectation(new Expectation<T>(expectation, description));
                return this;
            }

            public ExpectingAnotherThenOrWireup And<T>(Expression<Func<T, bool>> expectation)
            {
                _instance._expectations.AddExpectation(new Expectation<T>(expectation));
                return this;
            }

            public MessageBasedSpecification Wireup(Func<Action<object>, Router> setup)
            {
                return new ExpectingWireup(_instance).WireUp(setup);
            }

            public ExpectingAnotherThenOrWireup AndTotalNumberOfEventsIs(int count)
            {
                _instance._expectations.AddExpectation(new MessageCount(count));
                return this;
            }
        }

        public class ExpectingWireup
        {
            private readonly MessageBasedSpecification _instance;

            public ExpectingWireup(MessageBasedSpecification instance)
            {
                _instance = instance;
            }

            public MessageBasedSpecification WireUp(Func<Action<object>, Router> setup)
            {
                _instance._wireup = setup;
                return _instance;
            }
        }
    }
}