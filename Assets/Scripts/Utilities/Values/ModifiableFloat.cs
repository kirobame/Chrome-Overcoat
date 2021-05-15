using System;

namespace Chrome
{
    [Serializable]
    public class ModifiableFloat : ModifiableValue<float> 
    {
        public ModifiableFloat(float value) : base(value) { }
        
        public override IRegistry Copy()
        {
            var copy = new ModifiableFloat(Value);
            copy.Bootup();

            return copy;
        }
    }
}