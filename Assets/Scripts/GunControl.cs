using System;
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
        
        public GunPart Root => parts[0];

        public Transform Firepoint => firepoint;

        [SerializeField] private LayerMask rayMask;
        [SerializeField] private Transform raypoint;
        [SerializeField] private Transform firepoint;
        [SerializeReference] private GunPart[] parts = new GunPart[0];

        private PressState state;
        private float pressTime;
        
        void Awake()
        {
            state = PressState.Released;
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
            if (Input.GetMouseButtonDown(0) && state == PressState.Released)
            {
                if (!Root.IsActive) return;
                
                pressTime = 0.0f;
                Root.Start(ComputeAim(), EventArgs.Empty);

                state = PressState.Pressed;
            }
            
            if (Input.GetMouseButton(0) && state == PressState.Pressed)
            {
                pressTime += Time.deltaTime;
                Root.Update(ComputeAim(), EventArgs.Empty);
            }

            if (Input.GetMouseButtonUp(0) && state == PressState.Pressed)
            {
                Root.End(ComputeAim(), EventArgs.Empty);
                state = PressState.Released;
            }
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