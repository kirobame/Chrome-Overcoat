using System;
using Flux.Data;
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
        public Transform Firepoint => firepoint;

        [SerializeField] private Collider self;
        
        [Space, SerializeField] private Transform raypoint;
        [SerializeField] private Transform firepoint;
        
        [Space, SerializeField] private RemoteTaskTree leftWeapon;
        [SerializeField] private RemoteTaskTree rightWeapon;

        private bool isLeft;

        private Packet packet;
        private Blackboard blackboard;
        
        private PressState state;
        private float pressTime;
        
        void Awake()
        {
            blackboard = new Blackboard();
            blackboard.Set((byte)10, "type");
            blackboard.Set(raypoint, "view");
            blackboard.Set(firepoint, "view.fireAnchor");
            blackboard.Set(self.transform, "self");
            blackboard.Set(self, "self.collider");

            packet = new Packet();
            packet.Set(false);
            packet.Set(blackboard);
            
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

        void Update()
        {
            if (state == PressState.Released)
            {
                if (Input.GetKeyDown(KeyCode.E) && !isLeft) ChangeWeapon(true);
                if (Input.GetKeyDown(KeyCode.R) && isLeft) ChangeWeapon(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    pressTime = 0.0f;
                    packet.Set(true);

                    MoveControl.canSprint = false;
                    state = PressState.Pressed;
                }
            }
            else if (state == PressState.Pressed)
            {
                if (Input.GetMouseButton(0))
                {
                    pressTime += Time.deltaTime;
                    packet.Set(true);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    packet.Set(false);
                    
                    MoveControl.canSprint = true;
                    state = PressState.Released;
                }
            }

            Current.Update(packet);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;
            state = PressState.Pressed;
        }

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