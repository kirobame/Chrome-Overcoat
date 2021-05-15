using UnityEngine;

namespace Chrome
{
    public class ShieldUp : TaskNode
    {
        public ShieldUp(IValue<Shield> shield) => this.shield = shield;

        private IValue<Shield> shield;

        protected override void OnUse(Packet packet)
        {
            if (shield.IsValid(packet))
            {
                if (!shield.Value.IsBroken) shield.Value.Up();
            }
            
            isDone = true;
        }
    }
}
