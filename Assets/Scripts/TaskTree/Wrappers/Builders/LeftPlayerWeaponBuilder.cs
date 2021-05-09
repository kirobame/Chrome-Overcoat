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

            return new GunNode().Append(
                new Charge(1.0f, 2.5f, 2.0f).Mask(0b_0001),
                new CheckCharge(0.2f, 0.75f).Mask(0b_0010).Append( 
                    new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab).Mask(0b_0001)));
        }
    }
}