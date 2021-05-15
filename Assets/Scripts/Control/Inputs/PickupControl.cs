using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PickupControl : InputControl<PickupControl>
    {
        protected override void SetupInputs() => input.Value.BindKey(InputRefs.PICK_WP_02, this, key);

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private Weapon weapon;

        private Weapon runtimeWeapon;
        private IValue<GunControl> gun;
        
        private CachedValue<Key> key;
        private bool state;

        protected override void Awake()
        {
            key = new CachedValue<Key>(Key.Inactive);
            state = true;
            
            base.Awake();

           
            gun = injections.Register(new AnyValue<GunControl>());

            runtimeWeapon = Instantiate(weapon);
            runtimeWeapon.Build();
        }

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