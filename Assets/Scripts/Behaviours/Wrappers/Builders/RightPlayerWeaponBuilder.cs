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
        
        public RootNode Build()
        {
            var fireAnchorReference = "view.fireAnchor".Reference<Transform>();
            var colliderReference = "self.collider".Reference<Collider>();
            
            return new ClickInput().Append(
                new ComputeAimDirection("shootDir", LayerMask.GetMask("Environment", "Entity"), fireAnchorReference, "view".Reference<Transform>(), colliderReference).Mask(0b_0001).Append(
                    new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab).Append(
                        new Delay(delay))));
        }
    }
}