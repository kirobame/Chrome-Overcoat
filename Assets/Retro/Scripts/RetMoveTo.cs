using UnityEngine;
using UnityEngine.AI;

namespace Chrome.Retro
{
    public class RetMoveTo : TaskedNode
    {
        public RetMoveTo(IValue<NavMeshAgent> navMesh, IValue<Transform> target)
        {
            this.navMesh = navMesh;
            this.target = target;
        }
        public RetMoveTo(IValue<NavMeshAgent> navMesh, IValue<Transform> target, IValue<RetAgentLookAt> aim)
        {
            this.navMesh = navMesh;
            this.target = target;
            this.aim = aim;
        }
        
        private IValue<NavMeshAgent> navMesh;
        private IValue<Transform> target;
        private IValue<RetAgentLookAt> aim = new EmptyValue<RetAgentLookAt>();
        
        protected override void OnUse(Packet packet)
        {
            if (navMesh.IsValid(packet) && target.IsValid(packet))
            {
                navMesh.Value.updateRotation = true;
                navMesh.Value.isStopped = false;

                navMesh.Value.SetDestination(target.Value.position);

                if (aim.IsValid(packet)) aim.Value.direction = Vector3.forward;
            }
            
            isDone = true;
        }
    }
}