using System;
using System.Collections.Generic;
using System.Threading;

namespace dokimi.core.Specs.MessageBased
{
    public partial class MessageBasedSpecification : Specification
    {
        private readonly List<MessageEntry> _given = new List<MessageEntry>();
        private MessageEntry _when;
        private readonly Expectations _expectations = new Expectations();
        private Action<Action<object>> _sinkWireup;
        private Func<Action<object>, Router> _wireup;

        public void EnrichDescription(SpecInfo spec, MessageFormatter formatter)
        {
            foreach (var given in _given)
                given.DescribeTo(s => spec.ReportGivenStep(new StepInfo(s)), formatter);

            _when.DescribeTo(spec.ReportWhenStep, formatter);

            _expectations.DescribeTo(spec, formatter);
        }

        public SpecInfo Run(SpecInfo spec, MessageFormatter formatter)
        {
            var givenSteps = new StepInfo[_given.Count];

            for (int i = 0; i < _given.Count; i++)
            {
                var given = _given[i];
                int i1 = i;
                given.DescribeTo(s =>
                {
                    var stepInfo = new StepInfo(s);
                    givenSteps[i1] = stepInfo;
                    return spec.ReportGivenStep(stepInfo);
                }, formatter);
            }

            var prepareStep = new StepInfo("Setup completed");
            spec.ReportGivenStep(prepareStep);

            _when.DescribeTo(spec.ReportWhenStep, formatter);

            var token = new CancellationTokenSource();

            var events = new List<object>();

            try
            {
                //wireup
                Action<object> sink = events.Add;

                var router = _wireup(sink);

                if (_given.Count == 0)
                {
                    var step = new StepInfo("No history");
                    spec.ReportGivenStep(step);
                    step.Pass();
                }
                else for (int i = 0; i < _given.Count; i++)
                {
                    var g = _given[i];
                    g.RunTo(router);
                    givenSteps[i].Pass();
                }

                events.Clear();
                prepareStep.Pass();

                //run when
                _when.RunTo(router);
                spec.When.Pass();
            }
            catch
            {
                _expectations.DescribeTo(spec, formatter);
                throw;
            }
            finally
            {
                token.Cancel();
            }

            //then
            _expectations.Verify(events.ToArray(), spec, formatter);

            return spec;
        }

        public SpecificationCategory SpecificationCategory { get; private set; }

        protected MessageBasedSpecification(SpecificationCategory category)
        {
            SpecificationCategory = category;
        }

        public static ExpectingEvent New(SpecificationCategory category)
        {
            var instance = new MessageBasedSpecification(category);
            return new ExpectingEvent(instance);
        }

        public class MessageEntry
        {
            private readonly string _description;
            private readonly object _message;

            public MessageEntry(string description, object message)
            {
                _description = description;
                _message = message;
            }

            public void DescribeTo(Func<string, SpecInfo> func, MessageFormatter formatter)
            {
                func(_description ?? formatter.FormatMessage(_message));
            }

            public void RunTo(Router router)
            {
                router.Handle(_message);
            }
        }
    }
}