using UnityEngine;

namespace Chrome
{
    public class ConsumeAmmo : TaskNode
    {
        public ConsumeAmmo(float amount, IValue<float> ammo)
        {
            this.amount = amount;
            this.ammo = ammo;
        }
        
        private float amount;
        private IValue<float> ammo;
 
        protected override void OnUse(Packet packet)
        {
            if (ammo.IsValid(packet))
            {
                var current = ammo.Value;
                ammo.Value = current - amount;

                if (packet.TryGetValueAt<string, Token>(TokenRefs.ON_AMMO_CHANGE, out var token)) token.Consume();
            }
            
            isDone = true;
        }
    }
}