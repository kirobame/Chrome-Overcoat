using System;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class GunControl : InputControl
    {
        #region Nested Types

        private enum PressState
        {
            Pressed,
            Released,
        }

        #endregion

        public ITaskTree Current => isLeft ? leftWeapon : rightWeapon;

        [BoxGroup("Dependencies"), SerializeField] private PlayerBoard board;
        
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree leftWeapon;
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree rightWeapon;

        private Packet packet;
        
        private bool isLeft;
        private PressState state;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            packet = new Packet();
            packet.Set(false);
            packet.Set<IBlackboard>(board);
            
            state = PressState.Released;
            
            leftWeapon.Bootup();
            rightWeapon.Bootup();

            isLeft = true;
            
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

            Current.Update(packet);
        }

        private void OnMouseDown()
        {
            board.Get<BusyBool>("canSprint").business++;
            state = PressState.Pressed;
        }
        private void OnMouseUp()
        {
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