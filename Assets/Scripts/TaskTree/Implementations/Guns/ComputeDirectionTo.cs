using System;
using System.Linq;
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

        private IValue<Vector3> direction;
        private IValue<Transform> from;
        private IValue<Collider> target;

        protected override void OnUse(Packet packet)
        {
            if (direction.IsValid(packet) && from.IsValid(packet) && target.IsValid(packet))
            {
                var mask = LayerMask.GetMask("Environment");
                var corners = target.Value.bounds.GetCorners().ToList();

                var point = Vector3.zero;
                for (var i = 0; i < corners.Count; i++)
                {
                    if (from.Value.position.CanSee(corners[i], mask))
                    {
                        point += corners[i];
                        continue;
                    }
                
                    corners.RemoveAt(i);
                    i--;
                }

                if (corners.Any())
                {
                    point /= corners.Count;
                    
                    var nudge = target.Value.bounds.center - point;
                    point += nudge * 0.33f;
                    
                    direction.Value = Vector3.Normalize(point - from.Value.position);
                }
                else direction.Value = Vector3.Normalize(target.Value.bounds.center - from.Value.position);
            }

            isDone = true;
        }
    }
}