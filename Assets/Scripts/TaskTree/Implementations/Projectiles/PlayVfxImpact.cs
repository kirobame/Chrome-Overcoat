using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class PlayVfxImpact : TaskNode
    {
        public PlayVfxImpact(PoolableVfx impactVfx) => this.impactVfx = impactVfx;
        
        private PoolableVfx impactVfx;
        
        protected override void OnUse(Packet packet)
        {
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfxPoolable = vfxPool.RequestSinglePoolable(impactVfx);
            vfxPoolable.transform.localScale = Vector3.one;
            
            var board = packet.Get<IBlackboard>();
            var hit = board.Get<CollisionHit<Transform>>("hit");
            if (hit.Collider.gameObject.layer == LayerMask.NameToLayer("Entity")) vfxPoolable.transform.SetParent(hit.Collider.transform);
            
            vfxPoolable.transform.position = hit.Point;
            vfxPoolable.transform.rotation = Quaternion.LookRotation(hit.Normal);
            vfxPoolable.Value.Play();

            isDone = true;
        }
    }
}