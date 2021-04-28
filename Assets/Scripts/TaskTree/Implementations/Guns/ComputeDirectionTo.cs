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
        
        [SerializeField] private string path;

        private IValue<Transform> from;
        private IValue<Collider> target;

        protected override void OnUpdate(Packet packet)
        {
            if (from.IsValid(packet) && target.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();

                var point = target.Value.bounds.center;
                board.Set(path, Vector3.Normalize(point - from.Value.position));
            }

            isDone = true;
        }
    }
}