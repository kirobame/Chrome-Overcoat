using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class UMPWeaponBuilder : WeaponBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        [SerializeField] private float delay;
        
        public override ITaskTree Build()
        {
            base.Build();
            
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var colliderRef = Refs.COLLIDER.Reference<Collider>();

            return new GunNode().Append
            (
                TT.BIND_TO(WeaponRefs.ON_MOUSE_DOWN, TT.IF(new HasAmmo(ammoBinding))).Append
                (
                    new Shoot("shootDir".Reference<Vector3>(), fireAnchorRef, colliderRef, bulletPrefab, muzzleFlashPrefab).Append
                    (
                        new ConsumeAmmo(1.0f, ammoBinding),
                        new Delay(delay)
                    )
                )
            );
        }
    }
}