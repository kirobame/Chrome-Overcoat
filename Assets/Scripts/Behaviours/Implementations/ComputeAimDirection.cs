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
        
        [SerializeField] private LayerMask mask;
        [SerializeField] private string path;

        private IValue<Transform> from;
        private IValue<Transform> view;

        protected override void OnUpdate(Packet packet)
        {
            if (from.IsValid(packet) && view.IsValid(packet))
            {
                var board = packet.Get<Blackboard>();

                var length = 450.0f;
                var ray = new Ray(view.Value.position, view.Value.forward);

                Vector3 point;
                
                if (Physics.Raycast(ray, out var hit, length, mask)) point = hit.point;
                else point = ray.origin + ray.direction * length;
                
                board.Set(point, path);
            }

            IsDone = true;
        }
    }
}