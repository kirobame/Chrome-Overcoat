using UnityEngine.AI;

namespace Chrome
{
    public class StopMoving : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            if (packet.TryGet<NavMeshAgent>(out var navMesh))
            {
                navMesh.updateRotation = false;
                navMesh.isStopped = true;
            }
            
            isDone = true;
        }
    }
}