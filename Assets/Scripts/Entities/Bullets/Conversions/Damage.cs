using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class Damage : ProxyNode
    {
        public Damage(float amount, PoolableVfx impactVfx)
        {
            this.amount = amount;
            this.impactVfx = impactVfx;
        }

        private float amount;
        private PoolableVfx impactVfx;
        
        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            var identity = packet.Get<IIdentity>();
            
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfxPoolable = vfxPool.RequestSinglePoolable(impactVfx);
            vfxPoolable.transform.localScale = Vector3.one;
            
            var hit = board.Get<CollisionHit<Transform>>("hit");
            if (hit.Collider.TryGetComponent<InteractionHub>(out var hub))
            {
                packet.Set(hit);
                    
                hub.Relay<IDamageable>(damageable =>
                {
                    if (damageable.Identity.Faction == identity.Faction) return;
                    damageable.Hit(identity, amount, packet);
                });
                    
                vfxPoolable.transform.SetParent(hit.Collider.transform);
            }

            vfxPoolable.transform.position = hit.Point;
            vfxPoolable.transform.rotation = Quaternion.LookRotation(hit.Normal);
            vfxPoolable.Value.Play();

            isDone = true;
        }
    }
}