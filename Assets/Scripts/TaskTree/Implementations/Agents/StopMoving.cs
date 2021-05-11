using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class StopMoving : TaskedNode
    {
        public StopMoving(IValue<NavMeshAgent> navMesh) => this.navMesh = navMesh;
        
        private IValue<NavMeshAgent> navMesh;
        
        protected override void OnUse(Packet packet)
        {
            if (navMesh.IsValid(packet))
            {
                navMesh.Value.updateRotation = false;
                navMesh.Value.isStopped = true;
            }
            
            isDone = true;
        }
    }
}