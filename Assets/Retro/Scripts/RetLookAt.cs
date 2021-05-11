using UnityEngine;

namespace Chrome.Retro
{
    public class RetLookAt : TaskedNode
    {
        public RetLookAt(IValue<Collider> target, IValue<RetAgentLookAt> aim)
        {
            this.target = target;
            this.aim = aim;
        }
        
        private IValue<Collider> target;
        private IValue<RetAgentLookAt> aim;

        protected override void OnUse(Packet packet)
        {
            if (target.IsValid(packet) && aim.IsValid(packet))
            {
                var direction = Vector3.Normalize(target.Value.bounds.center.Flatten() - aim.Value.transform.position.Flatten());
                aim.Value.direction = aim.Value.transform.parent.InverseTransformDirection(direction);
            }
            
            isDone = true;
        }
    }
}