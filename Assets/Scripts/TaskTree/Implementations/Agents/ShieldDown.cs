using UnityEngine;

namespace Chrome
{
    public class ShieldDown : ProxyNode
    {
        public ShieldDown(IValue<GameObject> shield)
        {
            this.shield = shield;
        }

        private IValue<GameObject> shield;

        protected override void OnUpdate(Packet packet)
        {
            if (shield.IsValid(packet))
            {
                if (shield.Value.activeSelf)
                    shield.Value.SetActive(false);
            }

            isDone = true;
        }
    }
}
