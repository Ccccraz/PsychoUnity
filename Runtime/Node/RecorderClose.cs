using Unity.VisualScripting;

namespace PsychoUnity.Node
{
    [UnitCategory("PsychoUnity\\Recorder")]
    public class RecorderClose : Unit
    {
        [DoNotSerialize] private ControlOutput OutputTrigger { get; set; }
        
        [DoNotSerialize] private ValueInput Recorder { get; set; }
        
        protected override void Definition()
        {
            Recorder = ValueInput(nameof(Recorder), string.Empty);
            
            ControlInput(" ", flow =>
            {
                PsychoUnity.RecorderIns.Destroy(flow.GetValue<string>(Recorder));
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(string.Empty);
        }
    }
}
