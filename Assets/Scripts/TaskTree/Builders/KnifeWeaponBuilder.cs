using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class KnifeWeaponBuilder : TreeBuilder, IWeaponBuilder
    {
        [SerializeField] private float damage;
        [SerializeField] private Vector3 configuration;
        [SerializeField] private float delay;
        [SerializeField] private float cooldown;
        [SerializeField] private PoolableVfx hitVfxPrefab;
        
        public override ITaskTree Build()
        {
            var colliderRef = Refs.COLLIDER.Reference<Collider>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            
            return new GunNode().Append
            (
                TT.BIND_TO(WeaponRefs.ON_MOUSE_UP, new Attack(colliderRef, pivotRef, damage, configuration, delay, cooldown, hitVfxPrefab))
            );
        }

        public void InstallDependenciesOn(IBlackboard board) { }
        public IBindable[] GetBindables() => Array.Empty<IBindable>();
    }
}