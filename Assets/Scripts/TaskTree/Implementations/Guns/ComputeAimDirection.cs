using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ComputeAimDirection : TaskNode
    {
        public ComputeAimDirection(IValue<Vector3> direction, LayerMask mask, IValue<Transform> from, IValue<Transform> view)
        {
            this.direction = direction;
            this.mask = mask;

            this.from = from;
            this.view = view;
        }
        public ComputeAimDirection(IValue<Vector3> direction, LayerMask mask, IValue<Transform> from, IValue<Transform> view, IValue<Collider> collider)
        {
            this.direction = direction;
            this.mask = mask;

            this.from = from;
            this.view = view;
            this.collider = collider;
        }
        
        [SerializeField] private LayerMask mask;

        private IValue<Vector3> direction;
        private IValue<Transform> from;
        private IValue<Transform> view;
        private IValue<Collider> collider = new EmptyValue<Collider>();

        protected override void OnUse(Packet packet)
        {
            if (direction.IsValid(packet) && from.IsValid(packet) && view.IsValid(packet))
            {
                var length = 450.0f;
                var ray = new Ray(view.Value.position, view.Value.forward);
                Vector3 point;
                
                if (Physics.Raycast(ray, out var hit, length, mask))
                {
                    if (collider.IsValid(packet) && hit.collider == collider.Value)
                    {
                        ray = new Ray(hit.point + ray.direction * 0.01f, ray.direction);
                        
                        if (Physics.Raycast(ray, out hit, length, mask)) point = hit.point;
                        else point = ray.origin + ray.direction * length;
                    }                    
                    else point = hit.point;
                }
                else point = ray.origin + ray.direction * length;

                direction.Value = Vector3.Normalize(point - from.Value.position);
            }

            isDone = true;
        }
    }
}