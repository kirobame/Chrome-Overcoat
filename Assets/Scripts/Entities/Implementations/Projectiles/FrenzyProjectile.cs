using System.Collections;
using System.Collections.Generic;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class FrenzyProjectile : StandardProjectile
    {
        [FoldoutGroup("Values"), SerializeField] private float damage;
        [FoldoutGroup("Bounce"), SerializeField] private PoolableVfx impactVfx;
        [FoldoutGroup("Bounce"), SerializeField, Range(0.01f, 3.0f)] private float impactSize;
        protected override void OnHit(RaycastHit hit)
        {
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfx = PlayImpact(vfxPool.RequestSinglePoolable(impactVfx), hit);
            TryHit(vfx, hit);
        }

        private void Explode()
        {

        }
        private PoolableVfx PlayImpact(PoolableVfx vfx, RaycastHit hit)
        {
            vfx.transform.localScale = Vector3.one * (transform.localScale.x * impactSize);
            vfx.transform.position = hit.point;
            vfx.transform.rotation = Quaternion.LookRotation(hit.normal);

            vfx.Value.Play();
            return vfx;
        }
        private void TryHit(PoolableVfx vfx, RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<InteractionHub>(out var hub))
            {
                identity.Packet.Set(hit);

                hub.Relay<IDamageable>(damageable =>
                {
                    if (damageable.Identity.Faction == identity.Faction) return;
                    damageable.Hit(identity, damage, identity.Packet);
                });

                vfx.transform.SetParent(hit.transform);
            }
        }
    }
}
