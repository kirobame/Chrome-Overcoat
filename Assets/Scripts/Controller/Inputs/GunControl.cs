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

        public RootNode Current => isLeft ? leftWeapon.Root : rightWeapon.Root;
        public Transform Firepoint => firepoint;

        [SerializeField] private LayerMask rayMask;
        [SerializeField] private Transform raypoint;
        [SerializeField] private Transform firepoint;
        [SerializeField] private RemoteTaskTree leftWeapon;
        [SerializeField] private RemoteTaskTree rightWeapon;

        private bool isLeft;

        private Packet packet;
        private Blackboard blackboard;
        
        private PressState state;
        private float pressTime;
        
        void Awake()
        {
            blackboard = new Blackboard();
            blackboard.Set(raypoint, "view");
            blackboard.Set(firepoint, "view.fireAnchor");
            
            packet = new Packet();
            packet.Set(false);
            packet.Set(blackboard);
            
            state = PressState.Released;
            
            leftWeapon.Bootup();
            rightWeapon.Bootup();

            isLeft = true;
        }

        void Update()
        {
            if (state == PressState.Released)
            {
                if (Input.GetKeyDown(KeyCode.E) && !isLeft) ChangeIndex(true);
                if (Input.GetKeyDown(KeyCode.R) && isLeft) ChangeIndex(false);
                
                if (Input.GetMouseButtonDown(0))
                {
                    pressTime = 0.0f;
                    Current.Start(packet);

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

        private void ChangeIndex(bool value)
        {
            isLeft = value;
            
            var HUD = Repository.Get<GunHUD>(Interface.Gun);
            HUD.Select(isLeft ? 0 : 1);
        }

        private Aim ComputeAim()
        {
            var length = 450.0f;
            var ray = new Ray(raypoint.position, raypoint.forward);

            Vector3 endPoint;
            if (Physics.Raycast(ray, out var hit, length, rayMask)) endPoint = hit.point;
            else endPoint = ray.origin + ray.direction * length;
            
            return new Aim()
            {
                direction = Vector3.Normalize(endPoint - firepoint.position),
                
                firepoint = firepoint.position,
                firingDirection = firepoint.forward,
                
                pressTime = pressTime
            };
        }
    }
}