using System;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class GunControl : InputControl, ILink<IIdentity>
    {
        #region Nested Types

        private enum PressState
        {
            Pressed,
            Released,
        }

        #endregion
        
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        public ITaskTree Current => isLeft ? leftWeapon : rightWeapon;

        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree leftWeapon;
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree rightWeapon;
        
        private bool isLeft;
        private PressState state;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            identity.Packet.Set(false);

            state = PressState.Released;
            
            leftWeapon.Bootup();
            rightWeapon.Bootup();

            isLeft = true;
            
            Current.Bootup(identity.Packet);
        }

        public override void Bootup()
        {
            identity.Packet.Set(false);
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
            if (state == PressState.Released)
            {
                if (Input.GetKeyDown(KeyCode.E) && !isLeft) ChangeWeapon(true);
                if (Input.GetKeyDown(KeyCode.R) && isLeft) ChangeWeapon(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    identity.Packet.Set(true);
                    OnMouseDown();
                }
            }
            else if (state == PressState.Pressed)
            {
                if (Input.GetMouseButton(0)) identity.Packet.Set(true);

                if (Input.GetMouseButtonUp(0))
                {
                    identity.Packet.Set(false);
                    OnMouseUp();
                }
            }

            Current.Update(identity.Packet);
        }

        private void OnMouseDown()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Get<BusyBool>("canSprint").business++;
            
            state = PressState.Pressed;
        }
        private void OnMouseUp()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Get<BusyBool>("canSprint").business--;
            
            state = PressState.Released;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private void ChangeWeapon(bool value)
        {
            Current.Shutdown(identity.Packet);
            isLeft = value;
            
            var HUD = Repository.Get<GunHUD>(Interface.Gun);
            HUD.Select(isLeft ? 0 : 1);
            
            Current.Bootup(identity.Packet);
        }
    }
}