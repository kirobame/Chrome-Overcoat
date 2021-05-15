using UnityEngine;

namespace Chrome
{
    public class LookAt : TaskNode
    {
        public LookAt(IValue<Collider> target, IValue<Transform> aim)
        {
            this.target = target;
            this.aim = aim;
        }
        
        private IValue<Collider> target;
        private IValue<Transform> aim;
        
        protected override void OnUse(Packet packet)
        {
            if (target.IsValid(packet) && aim.IsValid(packet))
            {
                var point = target.Value.bounds.center;
                aim.Value.LookAt(point);
            }
            
            isDone = true;
        }
    }
}