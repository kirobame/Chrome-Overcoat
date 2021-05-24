using System;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class WeaponLoot : Loot, IRebootable, IPopupInfo
    {
        string IPopupInfo.Text => $"Pickup {value.name}";
        
        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private Weapon value;
        [FoldoutGroup("Values"), SerializeField] private float decrease;

        [FoldoutGroup("HUD"), SerializeField] private Transform anchor;
        [FoldoutGroup("HUD"), SerializeField] private GenericPoolable worldHUDPrefab;

        private bool hasWorldHUD;
        private WorldAmmoHUD worldHUD;
        private Weapon runtimeValue;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            runtimeValue = Instantiate(value);
            runtimeValue.Build();
        }

        void Update()
        {
            if (!lifetime.Value.IsAlive || !runtimeValue.Board.TryGet<BindableGauge>(WeaponRefs.AMMO, out var ammoBinding)) return;

            ammoBinding.Value -= decrease * Time.deltaTime;
            if (ammoBinding.IsAtMin) lifetime.Value.End();
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void Reboot()
        {
            if (!runtimeValue.Board.TryGet<BindableGauge>(WeaponRefs.AMMO, out var ammoBinding)) return;
            ammoBinding.Value = ammoBinding.Range.y;
        }

        public override void Bootup(byte code)
        {
            base.Bootup(code);

            if (!runtimeValue.Board.TryGet<BindableGauge>(WeaponRefs.AMMO, out var ammoBinding))
            {
                hasWorldHUD = false;
                return;
            }
            
            var worldHUDPool = Repository.Get<GenericPool>(Pool.WorldHUD);
            worldHUD = worldHUDPool.CastSingle<WorldAmmoHUD>(worldHUDPrefab);
            
            worldHUD.Bind(anchor, ammoBinding);
            hasWorldHUD = true;
        }
        public override void Shutdown(byte code)
        {
            base.Shutdown(code);
            if (hasWorldHUD) worldHUD.Discard();
        }

        //--------------------------------------------------------------------------------------------------------------/

        protected override void OnPickup(IIdentity source)
        {
            if (!source.Packet.TryGet<GunControl>(out var gunControl)) return;
            gunControl.SwitchTo(runtimeValue);
        }
    }
}