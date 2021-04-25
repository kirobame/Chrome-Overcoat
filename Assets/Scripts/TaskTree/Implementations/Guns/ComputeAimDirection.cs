using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ComputeAimDirection : ProxyNode
    {
        public ComputeAimDirection(string path, LayerMask mask, IValue<Transform> from, IValue<Transform> view)
        {
            this.path = path;
            this.mask = mask;

            this.from = from;
            this.view = view;
        }
        public ComputeAimDirection(string path, LayerMask mask, IValue<Transform> from, IValue<Transform> view, IValue<Collider> collider)
        {
            this.path = path;
            this.mask = mask;

            this.from = from;
            this.view = view;
            this.collider = collider;
        }
        
        [SerializeField] private LayerMask mask;
        [SerializeField] private string path;
        
        private IValue<Transform> from;
        private IValue<Transform> view;
        private IValue<Collider> collider = new EmptyValue<Collider>();

        protected override void OnUpdate(Packet packet)
        {
            if (from.IsValid(packet) && view.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();

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

                board.Set(path, point - from.Value.position);
            }

            isDone = true;
        }
    }
}