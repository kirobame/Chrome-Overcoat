using System;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class GunControl : InputControl<GunControl>, IInjectable
    {
        #region Nested Types

        private enum PressState
        {
            Pressed,
            Released,
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------/

        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/

        public ITaskTree Current => isLeft ? leftWeapon : rightWeapon;

        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree leftWeapon;
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree rightWeapon;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;
        
        private bool isLeft;
        private PressState state;

        private ComputeAimDirection aimCompute;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };

            packet.Set(false);

            state = PressState.Released;
            isLeft = true;
            
            leftWeapon.Bootup();
            rightWeapon.Bootup();
            aimCompute = ChromeExtensions.CreateComputeAimDirection();
            
            Current.Bootup(packet);
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
                if (Input.GetKeyDown(KeyCode.E) && !isLeft) ChangeWeapon(true);
                if (Input.GetKeyDown(KeyCode.R) && isLeft) ChangeWeapon(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    packet.Set(true);
                    OnMouseDown();
                }
            }
            else if (state == PressState.Pressed)
            {
                if (Input.GetMouseButton(0)) packet.Set(true);

                if (Input.GetMouseButtonUp(0))
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
            board.Get<BusyBool>("canSprint").business++;
            
            state = PressState.Pressed;
        }
        private void OnMouseUp()
        {
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>("canSprint").business--;
            
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