using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class FrenzyPlayerWeaponBuilder : ITreeBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;

        public ITaskTree Build()
        {
            var fireAnchorReference = "view.fireAnchor".Reference<Transform>();
            var colliderReference = "self.collider".Reference<Collider>();

            var gauge = Repository.Get<Gauge>(Gauges.One);
            var costLock = "frenzy.heatLock".Reference<bool>();
            var cost = "frenzy.heatCost".Reference<float>();

            return new FrenzyAbilityNode().Append(
                new GaugeCost(costLock, cost, gauge).Append( 
                    new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab).Mask(0b_0001)));
        }
    }
}
