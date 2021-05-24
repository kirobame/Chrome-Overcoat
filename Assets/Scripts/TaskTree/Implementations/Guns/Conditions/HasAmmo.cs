using System;

namespace Chrome
{
    [Serializable]
    public class HasAmmo : Condition
    {
        public HasAmmo(Bindable<float> binding) => this.binding = binding;

        private Bindable<float> binding;
        
        public override bool Check(Packet packet) => binding.Value > 0.0f;
    }
}