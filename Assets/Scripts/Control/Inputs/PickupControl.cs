using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PickupControl : InputControl<PickupControl>
    {
        protected override void PrepareInjection()
        {
            state = true;
            
            runtimeWeapon = Instantiate(weapon);
            runtimeWeapon.Build();
            
            gun = injections.Register(new AnyValue<GunControl>());
        }

        protected override void SetupInputs()
        {
            key = new CachedValue<Key>(Key.Inactive);
            input.Value.BindKey(InputRefs.PICK_WP_02, this, key);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private Weapon weapon;

        private Weapon runtimeWeapon;
        private IValue<GunControl> gun;
        
        private CachedValue<Key> key;
        private bool state;
        
        void Update()
        {
            if (!key.IsDown()) return;

            if (!state)
            {
                gun.Value.DropCurrent();
                state = true;
            }
            else
            {
                gun.Value.SwitchTo(runtimeWeapon);
                state = false;
            }
        }
    }
}