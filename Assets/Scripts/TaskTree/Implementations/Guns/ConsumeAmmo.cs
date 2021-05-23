using UnityEngine;

namespace Chrome
{
    public class ConsumeAmmo : TaskNode
    {
        public ConsumeAmmo(float amount, Bindable<float> binding)
        {
            this.amount = amount;
            this.binding = binding;
        }
        
        private float amount;
        private Bindable<float> binding;
 
        protected override void OnUse(Packet packet)
        {
            binding.Value -= amount;
            isDone = true;
        }
    }
}