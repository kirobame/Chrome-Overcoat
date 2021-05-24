using System;
using Flux.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class SubmachineGunWeaponBuilder : ITreeBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        [SerializeField] private int burstAmmount;
        [SerializeField] private float fireRateDelay;
        [SerializeField] private float burstDelay;

        public ITaskTree Build()
        {
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var colliderRef = Refs.COLLIDER.Reference<Collider>();

            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);

            return new GunNode().Append
            (
                new Shoot("shootDir".Reference<Vector3>(), fireAnchorRef, colliderRef, bulletPrefab, muzzleFlashPrefab).Append
                (
                    new BurstCountNode(),
                    new Delay(fireRateDelay)/*.Append
                    (
                        new BulletBurst()
                    )*/
                )

            ) ;
            /*
            return new GunNode().Append
            (
                TT.IF(new BulletBurst()).Append
                (
                    TT.IF_TRUE(new Shoot("shootDir".Reference<Vector3>(), fireAnchorRef, colliderRef, bulletPrefab, muzzleFlashPrefab)).Append
                    (
                        new Delay(fireRateDelay)
                    ),
                    TT.IF_FALSE(new Delay(burstDelay))
                )
            );*/

            /*
            return new GunNode().Append
            (
                TT.BIND_TO(0b_0001, TT.IF(new BulletBurst(burstAmmount))).Append
                (
                    TT.IF_TRUE(new Shoot("shootDir".Reference<Vector3>(), fireAnchorRef, colliderRef, bulletPrefab, muzzleFlashPrefab)).Append
                    (
                        new Delay(fireRateDelay)
                    ),
                    TT.IF_FALSE(new Delay(burstDelay))
                )
            );
            */
        }
    }
}