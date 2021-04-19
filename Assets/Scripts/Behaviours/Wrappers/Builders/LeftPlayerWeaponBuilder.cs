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
        
        public RootNode Build()
        {
            var fireAnchorReference = "view.fireAnchor".Reference<Transform>();
            var colliderReference = "self.collider".Reference<Collider>();

            return new ClickInput().Append(
                new Charge(1.0f, 2.5f, 2.0f).Mask(0b_0001),
                new CheckCharge(0.2f, 0.75f).Mask(0b_0010).Append( 
                    new ComputeAimDirection("shootDir", LayerMask.GetMask("Environment", "Entity"), fireAnchorReference, "view".Reference<Transform>(), colliderReference).Mask(0b_0001).Append(
                        new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab))));
        }
    }
}