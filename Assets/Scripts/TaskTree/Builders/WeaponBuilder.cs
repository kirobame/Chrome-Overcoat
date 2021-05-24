using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class WeaponBuilder : TreeBuilder, IWeaponBuilder
    {
        [SerializeField] private float ammo;
        
        protected BindableGauge ammoBinding;
        
        public override ITaskTree Build()
        {
            ammoBinding = new BindableGauge(HUDBinding.Ammo, ammo,new Vector2(0.0f, ammo));
            return null;
        }

        public virtual void InstallDependenciesOn(IBlackboard board) => board.Set(WeaponRefs.AMMO, ammoBinding);
        public virtual IBindable[] GetBindables() => new IBindable[] { ammoBinding };
    }
}