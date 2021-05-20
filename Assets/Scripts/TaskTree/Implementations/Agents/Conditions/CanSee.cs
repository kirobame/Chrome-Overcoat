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
        public CanSee(IValue<Transform> source, IValue<Collider> target, IValue<Vector3> direction)
        {
            this.source = source;
            this.target = target;
            
            this.direction = direction;
        }

        private IValue<Transform> source;
        private IValue<Collider> target;
        
        private IValue<Vector3> direction = new EmptyValue<Vector3>();

        public override bool Check(Packet packet)
        {
            if (!source.IsValid(packet) || !target.IsValid(packet)) return false;

            var mask = LayerMask.GetMask("Environment");
            var corners = target.Value.bounds.GetCorners().ToList();

            var point = Vector3.zero;
            for (var i = 0; i < corners.Count; i++)
            {
                if (source.Value.CanSee(corners[i], mask))
                {
                    point += corners[i];
                    continue;
                }
                
                corners.RemoveAt(i);
                i--;
            }

            if (!corners.Any()) return false;
            
            point /= corners.Count;
            if (direction.IsValid(packet)) direction.Value = Vector3.Normalize(point - source.Value.position);

            return true;
        }
    }
}