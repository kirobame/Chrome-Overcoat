using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ComputeDirectionTo : TaskNode
    {
        public ComputeDirectionTo(IValue<Vector3> direction, IValue<Transform> from, IValue<Collider> target)
        {
            this. direction =  direction;
            this.from = from;
            this.target = target;
        }
        public ComputeDirectionTo(IValue<Vector3> direction, IValue<Transform> from, IValue<Collider> target, Vector3 offset)
        {
            this. direction =  direction;
            this.from = from;
            this.target = target;

            this.offset = offset;
        }

        private IValue<Vector3> direction;
        private IValue<Transform> from;
        private IValue<Collider> target;

        private Vector3 offset;

        protected override void OnUse(Packet packet)
        {
            if (direction.IsValid(packet) && from.IsValid(packet) && target.IsValid(packet))
            {
                var point = target.Value.bounds.center + offset;
                direction.Value = Vector3.Normalize(point - from.Value.position); 
            }

            isDone = true;
        }
    }
}