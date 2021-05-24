using UnityEngine;

namespace Chrome
{
    public class ResetAim : TaskNode
    {
        public ResetAim(IValue<Transform> aim) => this.aim = aim;
        
        private IValue<Transform> aim;

        protected override void OnUse(Packet packet)
        {
            if (aim.IsValid(packet)) aim.Value.localRotation = Quaternion.identity;
            isDone = true;
        }
    }
}