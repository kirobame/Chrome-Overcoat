using System;
using Flux.Data;
using Flux.Feedbacks;
using UnityEngine;

namespace Chrome
{
    public class GunControl : MonoBehaviour
    {
        #region Nested Types

        private enum PressState
        {
            Pressed,
            Released,
        }

        #endregion
        
        public GunPart[] Roots { get; private set; }

        public Transform Firepoint => firepoint;

        [SerializeField] private LayerMask rayMask;
        [SerializeField] private Transform raypoint;
        [SerializeField] private Transform firepoint;
        [SerializeReference] private GunPart[] parts = new GunPart[0];
        [SerializeReference] private GunPart[] otherParts = new GunPart[0];

        private int index;
        
        private PressState state;
        private float pressTime;
        
        void Awake()
        {
            state = PressState.Released;
            
            Initialize(parts);
            Initialize(otherParts);

            index = 0;
            Roots = new GunPart[] { parts[0], otherParts[0] };
        }

        private void Initialize(GunPart[] parts)
        {
            if (parts.Length <= 1) return;

            var buffer = new GunPart[1];
            for (var i = 0; i < parts.Length - 1; i++)
            {
                buffer[0] = parts[i + 1];
                
                parts[i].LinkTo(buffer);
                parts[i].Bootup(this);
            }
            
            parts[parts.Length - 1].Bootup(this);
        }

        void Update()
        {
            if (state == PressState.Released)
            {
                if (Input.GetKeyDown(KeyCode.E) && index != 0) ChangeIndex(0);
                if (Input.GetKeyDown(KeyCode.R) && index != 1) ChangeIndex(1);
                
                if (Input.GetMouseButtonDown(0))
                {
                    if (!Roots[index].IsActive) return;
                
                    pressTime = 0.0f;
                    Roots[index].Start(ComputeAim(), EventArgs.Empty);

                    MoveControl.canSprint = false;
                    state = PressState.Pressed;
                }
            }
            else if (state == PressState.Pressed)
            {
                if (Input.GetMouseButton(0))
                {
                    pressTime += Time.deltaTime;
                    Roots[index].Update(ComputeAim(), EventArgs.Empty);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Roots[index].End(ComputeAim(), EventArgs.Empty);

                    MoveControl.canSprint = true;
                    state = PressState.Released;
                }
            }
        }

        private void ChangeIndex(int value)
        {
            index = value;
            
            var HUD = Repository.Get<GunHUD>(Interface.Gun);
            HUD.Select(value);
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