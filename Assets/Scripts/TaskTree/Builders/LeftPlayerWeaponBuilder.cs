using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class LeftPlayerWeaponBuilder : ITreeBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        
        public ITaskTree Build()
        {
            var fireAnchorReference = Refs.FIREANCHOR.Reference<Transform>();
            var colliderReference = Refs.COLLIDER.Reference<Collider>();
            var ammoRef = $"{WeaponRefs.BOARD}.{WeaponRefs.AMMO}".Reference<float>(ReferenceType.Nested);

            return new GunNode().Append
            (
                TT.BIND_TO(0b_0001, new Charge(1.0f, 2.5f, 2.0f)),
                TT.BIND_TO(0b_0010, TT.IF(new CheckCharge(0.2f, 0.75f)).AND(new HasAmmo(ammoRef))).Append
                (
                    TT.IF_TRUE(new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab)).Append
                    (
                        new ConsumeAmmo(5.0f, ammoRef)
                    )
                )
            );
        }
    }
}