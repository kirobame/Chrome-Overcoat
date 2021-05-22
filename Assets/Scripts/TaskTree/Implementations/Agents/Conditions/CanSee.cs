using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class CanSee : Condition
    {
        public CanSee(IValue<Transform> source, IValue<Collider> target)
        {
            this.source = source;
            this.target = target;
        }

        private IValue<Transform> source;
        private IValue<Collider> target;

        public override bool Check(Packet packet)
        {
            if (!source.IsValid(packet) || !target.IsValid(packet)) return false;

            var mask = LayerMask.GetMask("Environment");
            var corners = target.Value.bounds.GetCorners().ToList();

            var point = Vector3.zero;
            for (var i = 0; i < corners.Count; i++)
            {
                if (source.Value.position.CanSee(corners[i], mask))
                {
                    point += corners[i];
                    continue;
                }
                
                corners.RemoveAt(i);
                i--;
            }

            return corners.Any();
        }
    }
}