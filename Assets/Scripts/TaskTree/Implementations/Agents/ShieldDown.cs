using UnityEngine;

namespace Chrome
{
    public class ShieldDown : ProxyNode
    {
        public ShieldDown(IValue<Shield> shield) => this.shield = shield;

        private IValue<Shield> shield;

        protected override void OnUpdate(Packet packet)
        {
            if (shield.IsValid(packet)) shield.Value.Down();
            isDone = true;
        }
    }
}
