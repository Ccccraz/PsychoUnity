using Unity.VisualScripting;
using UnityEngine;

namespace PsychoUnity.Node
{
    [TypeIcon(typeof(Unity.VisualScripting.Timer))]
    [UnitCategory("PsychoUnity")]
    public class Timer : Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;

        [DoNotSerialize] public ControlOutput OutputTrigger;

        [DoNotSerialize] public ValueInput EventName;
        
        [DoNotSerialize] public ValueInput Duration;
        
        [DoNotSerialize] public ValueInput Delay;
        
        [DoNotSerialize] public ValueInput Times;
        
        protected override void Definition()
        {
            EventName = ValueInput<string>("Event", string.Empty);
            Duration = ValueInput<int>("Duration", 0);
            Delay = ValueInput<int>("Delay", 0);
            Times = ValueInput<int>("Times", 1);

            InputTrigger = ControlInput(" ", flow =>
            {
                var eventName = flow.GetValue<string>(EventName);
                var duration = flow.GetValue<int>(Duration);
                var delay = flow.GetValue<int>(Delay);
                var times = flow.GetValue<int>(Times);
                
                PsychoUnity.Timer.Timing(duration, delay, times, () => EventBus.Trigger(eventName, 0));
                
                return OutputTrigger;
            });
            
            OutputTrigger = ControlOutput(" ");
        }
    }
}
