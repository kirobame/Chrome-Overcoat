using System;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class GunControl : InputControl<GunControl>
    {
        #region Nested Types

        private enum PressState
        {
            Pressed,
            Released,
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void SetupInputs()
        {
            input.Value.Bind(InputRefs.PICK_WP_01, this, OnPickWeapon01Input);
            input.Value.Bind(InputRefs.PICK_WP_02, this, OnPickWeapon02Input);
            input.Value.BindKey(InputRefs.SHOOT, this, shootKey);
        }
        void OnPickWeapon01Input(InputAction.CallbackContext context, InputCallbackType type)
        {
            if (state != PressState.Released) return;
            if (type == InputCallbackType.Cancelled && !isLeft) ChangeWeapon(true);
        }
        void OnPickWeapon02Input(InputAction.CallbackContext context, InputCallbackType type)
        {
            if (state != PressState.Released) return;
            if (type == InputCallbackType.Cancelled && isLeft) ChangeWeapon(false);
        }

        protected override void OnInjectionDone(IRoot source)
        {
            packet.Set(false);
            Current.Bootup(packet);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public Weapon Current => isLeft ? runtimeWeapons[0] : runtimeWeapons[1];

        [FoldoutGroup("Values"), SerializeField] private Weapon leftWeapon;
        [FoldoutGroup("Values"), SerializeField] private Weapon rightWeapon;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;
        
        private bool isLeft;
        private PressState state;
        private CachedValue<Key> shootKey;

        private Weapon[] runtimeWeapons;
        private ComputeAimDirection aimCompute;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            shootKey = new CachedValue<Key>(Key.Inactive);
            
            base.Awake();
            
            identity = new AnyValue<IIdentity>();
            injections.Add(identity);

            state = PressState.Released;
            isLeft = true;
            
            runtimeWeapons = new Weapon[2]
            {
                Instantiate(leftWeapon),
                Instantiate(rightWeapon)
            };
            runtimeWeapons[0].Build();
            runtimeWeapons[1].Build();
            
            aimCompute = ChromeExtensions.CreateComputeAimDirection();
        }

        public override void Bootup()
        {
            packet.Set(false);
            base.Bootup();
        }
        public override void Shutdown()
        {
            base.Shutdown();
            if (state == PressState.Pressed) OnMouseUp();
        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;
            OnMouseDown();
        }
        
        void Update()
        {
            var snapshot = packet.Save();

            if (state == PressState.Released)
            {
                if (shootKey.IsDown())
                {
                    packet.Set(true);
                    OnMouseDown();
                }
            }
            else if (state == PressState.Pressed)
            {
                if (shootKey.IsActive()) packet.Set(true);

                if (shootKey.IsUp())
                {
                    packet.Set(false);
                    OnMouseUp();
                }
            }

            aimCompute.Use(packet);
            Current.Actualize(packet);

            packet.Load(snapshot);
        }

        private void OnMouseDown()
        {
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).business++;
            
            state = PressState.Pressed;
        }
        private void OnMouseUp()
        {
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).business--;
            
            state = PressState.Released;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private void ChangeWeapon(bool value)
        {
            Current.Shutdown(packet);
            isLeft = value;
            
            var HUD = Repository.Get<GunHUD>(Interface.Gun);
            HUD.Select(isLeft ? 0 : 1);
            
            Current.Bootup(packet);
        }
    }
}