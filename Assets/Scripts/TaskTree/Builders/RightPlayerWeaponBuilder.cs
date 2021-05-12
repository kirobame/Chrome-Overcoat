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
            var fireAnchorReference = Refs.FIREANCHOR.Reference<Transform>();
            var colliderReference = Refs.COLLIDER.Reference<Collider>();
            
            return new GunNode().Append(
                new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab).Append(
                    new Delay(delay)).Mask(0b_0001));
        }
    }
}