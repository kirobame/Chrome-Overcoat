using UnityEngine;

namespace Chrome
{
    public class ShieldUp : ProxyNode
    {
        public ShieldUp(IValue<Shield> shield) => this.shield = shield;

        private IValue<Shield> shield;

        protected override void OnUpdate(Packet packet)
        {
            if (shield.IsValid(packet))
            {
                if (!shield.Value.IsBroken) shield.Value.Up();
            }
            
            isDone = true;
        }
    }
}
