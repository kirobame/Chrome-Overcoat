using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class MoveTo : ProxyNode
    {
        public MoveTo(IValue<NavMeshAgent> navMesh, IValue<Transform> target)
        {
            this.navMesh = navMesh;
            this.target = target;
        }
        public MoveTo(IValue<NavMeshAgent> navMesh, IValue<Transform> target, IValue<Transform> aim)
        {
            this.navMesh = navMesh;
            this.target = target;
            this.aim = aim;
        }
        
        private IValue<NavMeshAgent> navMesh;
        private IValue<Transform> target;
        private IValue<Transform> aim = new EmptyValue<Transform>();
        
        protected override void OnUpdate(Packet packet)
        {
            if (navMesh.IsValid(packet) && target.IsValid(packet))
            {
                navMesh.Value.updateRotation = true;
                navMesh.Value.isStopped = false;
                
                navMesh.Value.SetDestination(target.Value.position);
                
                if (aim.IsValid(packet)) aim.Value.localRotation = Quaternion.identity;
            }
            
            isDone = true;
        }
    }
}