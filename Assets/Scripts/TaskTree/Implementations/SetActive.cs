using UnityEngine;

namespace Chrome
{
    public class SetActive : TaskedNode
    {
        public SetActive(bool enabled, IValue<GameObject> target)
        {
            this.enabled = enabled;
            this.target = target;
        }
        
        private IValue<GameObject> target;
        private bool enabled;
        
        protected override void OnUse(Packet packet)
        {
            if (target.IsValid(packet)) target.Value.SetActive(enabled);
            isDone = true;
        }
    }
}