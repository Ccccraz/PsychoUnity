using System;
using Unity.VisualScripting;

namespace PsychoUnity.Node
{
    [UnitCategory("Events\\PsychoUnity")]
    public class CustomEvent : EventUnit<int>
    {
        [DoNotSerialize]
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput Name;

        public override EventHook GetHook(GraphReference reference)
        {
            var flow = Flow.New(reference);
            return new EventHook(flow.GetValue<string>(Name));
        }
    
        protected override void Definition()
        {
            base.Definition();
            
            Name = ValueInput<string>("name", string.Empty);
        }
    }
}