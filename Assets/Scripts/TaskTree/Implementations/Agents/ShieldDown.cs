using UnityEngine;

namespace Chrome
{
    public class ShieldDown : TaskNode
    {
        public ShieldDown(IValue<Shield> shield) => this.shield = shield;

        private IValue<Shield> shield;

        protected override void OnUse(Packet packet)
        {
            if (shield.IsValid(packet)) shield.Value.Down();
            isDone = true;
        }
    }
}
