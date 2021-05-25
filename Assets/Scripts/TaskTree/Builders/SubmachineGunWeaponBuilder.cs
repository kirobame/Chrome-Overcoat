using System;
using Flux.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class SubmachineGunWeaponBuilder : WeaponBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        [SerializeField] private int burstAmmount;
        [SerializeField] private float fireRateDelay;
        [SerializeField] private float burstDelay;

        public override ITaskTree Build()
        {
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var colliderRef = Refs.COLLIDER.Reference<Collider>();
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var shootDirRef = Refs.SHOOT_DIRECTION.Reference<Vector3>();

            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);

            return new GunNode().Append
            (
                new Shoot(shootDirRef, fireAnchorRef, colliderRef, bulletPrefab, muzzleFlashPrefab).Append
                (
                    new BurstCountNode(),
                    new Delay(fireRateDelay)
                )

            ) ;
        }
    }
}