using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PsychoUnity.Node
{
    /// <summary>
    /// A Timer based on asynchronous APIs can launch multiple tasks with a delayed start using a broadcast model.
    /// </summary>
    [TypeIcon(typeof(Unity.VisualScripting.Timer))]
    [UnitCategory("PsychoUnity")]
    public class Timer : Unit
    {
        /// <summary>
        /// Number of parameters
        /// </summary>
        [SerializeAs(nameof(ArgumentCount))]
        private int _argumentCount;

        /// <summary>
        /// Argument ports
        /// </summary>
        [DoNotSerialize] private List<ValueInput> Arguments { get; set; }

        /// <summary>
        /// Number of parameters
        /// </summary>
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Arguments")]
        public int ArgumentCount
        {
            get => _argumentCount;
            set => _argumentCount = Mathf.Clamp(value, 0, 10);
        }
        
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput Enter { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput Exit { get; private set; }
        
        /// <summary>
        /// The scope of influence of the Event
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput Target { get; private set; }

        /// <summary>
        /// Event name
        /// </summary>
        [DoNotSerialize] public ValueInput Event { get; private set; }
        
        /// <summary>
        /// The time interval of the Timer
        /// </summary>
        [DoNotSerialize] public ValueInput Duration { get; private set; }
        
        /// <summary>
        /// The amount of time to delay before the first timing starts, with a default of 0
        /// </summary>
        [DoNotSerialize] public ValueInput Delay { get; private set; }
        
        /// <summary>
        /// The number of times the timing needs to be repeated, The default is 1
        /// </summary>
        [DoNotSerialize] public ValueInput Times { get; private set; }
        
        protected override void Definition()
        {
            Target = ValueInput<GameObject>(nameof(Target), null).NullMeansSelf();
            
            Event = ValueInput(nameof(Event), string.Empty);
            Duration = ValueInput(nameof(Duration), 0);
            Delay = ValueInput(nameof(Delay), 0);
            Times = ValueInput(nameof(Times), 1);

            Arguments = new List<ValueInput>();

            for (var i = 0; i < ArgumentCount; i++)
            {
                var argument = ValueInput<object>($"Arg{i}");
                Arguments.Add(argument);
            }

            Enter = ControlInput(nameof(Enter), Trigger);
            
            Exit = ControlOutput(nameof(Exit));
        }

        private ControlOutput Trigger(Flow flow)
        {
            var eventName = flow.GetValue<string>(Event);
            var duration = flow.GetValue<int>(Duration);
            var delay = flow.GetValue<int>(Delay);
            var times = flow.GetValue<int>(Times);
            var target = flow.GetValue<GameObject>(Target);
            var arguments = Arguments.Select(flow.GetConvertedValue).ToArray();
            
            PsychoUnity.Timer.Timing(duration, delay, times,
                () => CustomEvent.Trigger(target, eventName, arguments));
            
            return Exit;
        }
    }
}
