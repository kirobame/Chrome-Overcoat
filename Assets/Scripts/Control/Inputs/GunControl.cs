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

        protected override void OnInjectionDone(IRoot source)
        {
            packet.Set(false);
            Current.Bootup(packet);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public ITaskTree Current => isLeft ? leftWeapon : rightWeapon;

        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree leftWeapon;
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree rightWeapon;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;
        
        private bool isLeft;
        private Key shootKey;
        private PressState state;

        private ComputeAimDirection aimCompute;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            shootKey = Key.Default;
            
            base.Awake();
            
            identity = new AnyValue<IIdentity>();
            injections.Add(identity);

            state = PressState.Released;
            isLeft = true;
            
            leftWeapon.Bootup();
            rightWeapon.Bootup();
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
        
        protected override void SetupInputs()
        {
            input.Value.Bind(InputRefs.PICK_WP_01, this, OnPickWeapon01Input);
            input.Value.Bind(InputRefs.PICK_WP_02, this, OnPickWeapon02Input);
            input.Value.Bind(InputRefs.SHOOT, this, OnShootInput, true);
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
        void OnShootInput(InputAction.CallbackContext context, InputCallbackType type) => shootKey.Update(type);

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
                if (shootKey.State == KeyState.Down)
                {
                    packet.Set(true);
                    OnMouseDown();
                }
            }
            else if (state == PressState.Pressed)
            {
                if (shootKey.State == KeyState.Active) packet.Set(true);

                if (shootKey.State == KeyState.Up)
                {
                    packet.Set(false);
                    OnMouseUp();
                }
            }

            aimCompute.Update(packet);
            Current.Update(packet);

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