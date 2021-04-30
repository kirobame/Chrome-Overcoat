using UnityEngine;
using UnityEngine.AI;

namespace Chrome.Retro
{
    public class RetMoveToCover : ProxyNode
    {
        public RetMoveToCover(IValue<NavMeshAgent> navMesh, IValue<RetCover> cover)
        {
            this.navMesh = navMesh;
            this.cover = cover;
        }
        public RetMoveToCover(IValue<NavMeshAgent> navMesh, IValue<RetCover> cover, IValue<Transform> aim)
        {
            this.navMesh = navMesh;
            this.cover = cover;
            this.aim = aim;
        }
        
        private IValue<NavMeshAgent> navMesh;
        private IValue<RetCover> cover;
        private IValue<Transform> aim = new EmptyValue<Transform>();

        private bool hasBeenBootedUp;

        protected override void Open(Packet packet)
        {
            Debug.Log("TEST");
            if (aim.IsValid(packet)) aim.Value.localRotation = Quaternion.identity;
        }

        protected override void OnStart(Packet packet)
        {
            if (!navMesh.IsValid(packet) || !cover.IsValid(packet)) return;
            
            navMesh.Value.updateRotation = true;
            navMesh.Value.isStopped = false;
            
            hasBeenBootedUp = false;
            navMesh.Value.SetDestination(cover.Value.transform.position.Flatten());
        }

        protected override void OnUpdate(Packet packet)
        {
            if (navMesh.IsValid(packet) && cover.IsValid(packet))
            {
                if (!hasBeenBootedUp)
                {
                    hasBeenBootedUp = true;
                    return;
                }
                else isDone = navMesh.Value.pathStatus == NavMeshPathStatus.PathComplete && navMesh.Value.remainingDistance <= 0.1f;
            }
            else isDone = true;
        }
    }
}