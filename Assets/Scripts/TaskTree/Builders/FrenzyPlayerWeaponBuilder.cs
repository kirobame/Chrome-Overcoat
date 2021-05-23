using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class FrenzyPlayerWeaponBuilder : TreeBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;

        public override ITaskTree Build()
        {
            var fireAnchorReference = Refs.FIREANCHOR.Reference<Transform>();
            var colliderReference = Refs.COLLIDER.Reference<Collider>();

            var gauge = Repository.Get<Gauge>(Gauges.One);
            var costLock = "frenzy.heatLock".Reference<bool>();
            var cost = "frenzy.heatCost".Reference<float>();

            return new FrenzyAbilityNode().Append(
                new GaugeCost(costLock, cost, gauge).Mask(0b_0001).Append( 
                    new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab).Mask(0b_0001)));
        }
    }
}
