using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class WeaponBuilder : TreeBuilder, IWeaponBuilder
    {
        [SerializeField] private float ammo;
        
        protected Bindable<float> ammoBinding;
        
        public override ITaskTree Build()
        {
            ammoBinding = new Bindable<float>(HUDBinding.Ammo, ammo);
            return null;
        }
//
        public override void Bootup(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            var weaponBoard = board.Get<IBlackboard>(WeaponRefs.BOARD);
            weaponBoard.Set(WeaponRefs.AMMO, ammoBinding);
        }

        public virtual IBindable[] GetBindables() => new IBindable[] { ammoBinding };
    }
}