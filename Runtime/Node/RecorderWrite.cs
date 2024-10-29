using Unity.VisualScripting;

namespace PsychoUnity.Node
{
    [UnitCategory("PsychoUnity\\Recorder")]
    public class RecorderWrite : Unit
    {
        [DoNotSerialize] private ControlOutput OutputTrigger { get; set; }

        [DoNotSerialize] private ValueInput Recorder { get; set; }
        
        protected override void Definition()
        {
            Recorder = ValueInput("Name", string.Empty);

            ControlInput(string.Empty, flow =>
            {
                PsychoUnity.RecorderIns.Write(flow.GetValue<string>(Recorder));
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(string.Empty);
        }
    }
}
