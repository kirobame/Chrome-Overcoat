using System;
using UnityEngine;

namespace Chrome
{
    public class GunControl : MonoBehaviour
    {
        public GunPart Root => parts[0];

        [SerializeField] private LayerMask rayMask;
        [SerializeField] private Transform raypoint;
        [SerializeField] private Transform firepoint;
        [SerializeReference] private GunPart[] parts = new GunPart[0];

        private float pressTime;
        
        void Awake()
        {
            if (parts.Length <= 1) return;

            var buffer = new GunPart[1];
            for (var i = 0; i < parts.Length - 1; i++)
            {
                buffer[0] = parts[i + 1];
                parts[i].LinkTo(buffer);
            }
        }

        void Update()
        {
            Debug.DrawRay(firepoint.position, firepoint.forward, Color.magenta);
            
            if (Input.GetMouseButtonDown(0))
            {
                pressTime = 0.0f;
                Root.Start(ComputeAim(), EventArgs.Empty);
            }
            
            if (Input.GetMouseButton(0))
            {
                pressTime += Time.deltaTime;
                Root.Update(ComputeAim(), EventArgs.Empty);
            }
            
            if (Input.GetMouseButtonUp(0)) Root.End(ComputeAim(), EventArgs.Empty);
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
                pressTime = pressTime
            };
        }
    }
}