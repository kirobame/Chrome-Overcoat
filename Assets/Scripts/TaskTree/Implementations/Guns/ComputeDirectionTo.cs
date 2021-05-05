using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ComputeDirectionTo : ProxyNode
    {
        public ComputeDirectionTo(string path, IValue<Transform> from, IValue<Collider> target)
        {
            this.path = path;

            this.from = from;
            this.target = target;
        }
        public ComputeDirectionTo(string path, IValue<Transform> from, IValue<Collider> target, Vector3 offset)
        {
            this.path = path;

            this.from = from;
            this.target = target;

            this.offset = offset;
        }
        
        [SerializeField] private string path;

        private IValue<Transform> from;
        private IValue<Collider> target;

        private Vector3 offset;

        protected override void OnUpdate(Packet packet)
        {
            if (from.IsValid(packet) && target.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();

                var point = target.Value.bounds.center + offset;
                board.Set(path, Vector3.Normalize(point - from.Value.position));
            }

            isDone = true;
        }
    }
}