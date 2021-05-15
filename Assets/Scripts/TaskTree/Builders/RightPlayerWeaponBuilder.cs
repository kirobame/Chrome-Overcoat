using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class RightPlayerWeaponBuilder : ITreeBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        [SerializeField] private float delay;
        
        public ITaskTree Build()
        {
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var colliderRef = Refs.COLLIDER.Reference<Collider>();
            var ammoRef = $"{WeaponRefs.BOARD}.{WeaponRefs.AMMO}".Reference<float>(ReferenceType.Nested);
            
            return new GunNode().Append(
                new Shoot("shootDir".Reference<Vector3>(), fireAnchorRef, colliderRef, bulletPrefab, muzzleFlashPrefab).Append(
                    new ConsumeAmmo(1.0f, ammoRef),
                    new Delay(delay)).Mask(0b_0001));
        }
    }
}