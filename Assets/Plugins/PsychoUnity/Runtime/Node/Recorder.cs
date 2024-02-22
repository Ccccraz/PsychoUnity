using System.Collections;
using Unity.VisualScripting;

namespace PsychoUnity.Node
{
    [UnitCategory("PsychoUnity\\Recorder")]
    public class Recorder : Unit
    {
        [DoNotSerialize] private ControlOutput OutputTrigger { get; set; }

        [DoNotSerialize] private ValueInput Data { get; set; }

        [DoNotSerialize] private ValueInput RecorderName { get; set; }
        
        [DoNotSerialize] private ValueInput Custom { get; set; }
        
        [DoNotSerialize] private ValueInput Prefix { get; set; }

        protected override void Definition()
        {
            Data = ValueInput<IDictionary>("Data", null);

            RecorderName = ValueInput("Name", string.Empty);
            
            Custom = ValueInput("Custom", string.Empty);
            
            Prefix = ValueInput("Prefix", string.Empty);

            ControlInput(" ", flow =>
            {
                var recorderName = flow.GetValue<string>(RecorderName);
                var data = flow.GetValue<IDictionary>(Data);
                var custom = flow.GetValue<string>(Custom);
                var prefix = flow.GetValue<string>(Prefix);
                PsychoUnity.Recorder.CreateRecorderNode(recorderName, data, custom, prefix);
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(string.Empty);
        }
    }
}
