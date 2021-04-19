using UnityEngine;

namespace Chrome
{
    public class LookAt : ProxyNode
    {
        public LookAt(IValue<PhysicBody> target, IValue<Transform> aim)
        {
            this.target = target;
            this.aim = aim;
        }
        
        private IValue<PhysicBody> target;
        private IValue<Transform> aim;
        
        protected override void OnUpdate(Packet packet)
        {
            if (target.IsValid(packet) && aim.IsValid(packet))
            {
                var point = target.Value.transform.position + target.Value.Controller.center;
                aim.Value.LookAt(point);
            }
            
            
            IsDone = true;
        }
    }
}