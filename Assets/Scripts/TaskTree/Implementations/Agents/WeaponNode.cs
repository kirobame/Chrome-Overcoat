using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Chrome
{
    [Serializable]
    public class WeaponNode : TaskNode
    {
        public WeaponNode(Weapon weapon)
        {
            this.weapon = Object.Instantiate(weapon);
            this.weapon.Build();
        }

        private Weapon weapon;

        protected override void OnBootup(Packet packet) => weapon.Bootup(packet);
        protected override void OnUse(Packet packet)
        {
            
            weapon.Actualize(packet);
        }
        protected override void OnShutdown(Packet packet) => weapon.Shutdown(packet);
    }
}