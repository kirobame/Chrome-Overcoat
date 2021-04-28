using UnityEngine.AI;

namespace Chrome
{
    public class StopMoving : ProxyNode
    {
        public StopMoving(IValue<NavMeshAgent> navMesh) => this.navMesh = navMesh;
        
        private IValue<NavMeshAgent> navMesh;
        
        protected override void OnUpdate(Packet packet)
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