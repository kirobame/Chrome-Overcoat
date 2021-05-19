using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class WeaponLoot : Loot, IRebootable
    {
        [FoldoutGroup("Values"), SerializeField] private Weapon value;

        private Weapon runtimeValue;

        void Awake()
        {
            runtimeValue = Instantiate(value);
            runtimeValue.Build();
        }

        public void Reboot()
        {
            // TODO : Implement ammo reset
        }

        public override void OnHoverStart(IIdentity source) => Debug.Log($"[O] Can pickup [{runtimeValue}]");
        public override void OnHoverEnd(IIdentity source) => Debug.Log($"[X] Cannot pickup [{runtimeValue}] anymore");

        protected override void OnPickup(IIdentity source)
        {
            if (!source.Packet.TryGet<GunControl>(out var gunControl)) return;
            gunControl.SwitchTo(runtimeValue);
        }
    }
}