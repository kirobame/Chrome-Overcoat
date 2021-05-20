using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chrome
{
    [Serializable]
    public class WeaponNode : TaskNode
    {
        public WeaponNode(IValue<Weapon> weapon) => this.weapon = weapon;

        private IValue<Weapon> weapon;

        protected override void OnBootup(Packet packet)
        {
            if (!weapon.IsValid(packet)) return;
            weapon.Value.Bootup(packet);
        }
        protected override void OnUse(Packet packet)
        {
            if (!weapon.IsValid(packet)) return;
            weapon.Value.Actualize(packet);
        }
        protected override void OnShutdown(Packet packet)
        {
            if (!weapon.IsValid(packet)) return;
            weapon.Value.Shutdown(packet);
        } 
    }
}