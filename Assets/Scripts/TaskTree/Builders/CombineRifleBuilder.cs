using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CombineRifleBuilder : WeaponBuilder
    {
        [SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        [SerializeField] private Vector3 configuration;

        private BindableCappedGauge gaugeBinding;
        
        public override ITaskTree Build()
        {
            base.Build();
            
            gaugeBinding = new BindableCappedGauge(HUDBinding.Gauge, new Vector2(0.0f, configuration.x), configuration.y);
            
            var fireAnchorReference = Refs.FIREANCHOR.Reference<Transform>();
            var colliderReference = Refs.COLLIDER.Reference<Collider>();
            var shootDirectionRef = Refs.SHOOT_DIRECTION.Reference<Vector3>();

            return new GunNode().Append
            (
                TT.BIND_TO(WeaponRefs.ON_MOUSE_DOWN, new Charge(gaugeBinding, 2.5f, 2.0f)),
                TT.BIND_TO(WeaponRefs.ON_MOUSE_UP, TT.IF(new CheckCharge(gaugeBinding, configuration.z)).AND(new HasAmmo(ammoBinding))).Append
                (
                    TT.IF_TRUE(new Shoot(shootDirectionRef, fireAnchorReference, colliderReference, bulletPrefab, muzzleFlashPrefab)).Append
                    (
                        new ConsumeAmmo(5.0f, ammoBinding)
                    )
                )
            );
        }

        public override void InstallDependenciesOn(IBlackboard board)
        {
            base.InstallDependenciesOn(board);
            board.Set(WeaponRefs.CHARGE, gaugeBinding);
        }
        public override IBindable[] GetBindables()
        {
            var output = base.GetBindables();
            CO.CONCAT(ref output, gaugeBinding);

            return output;
        }
    }
}