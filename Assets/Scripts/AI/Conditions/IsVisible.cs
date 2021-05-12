using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class IsVisible : Condition
    {
        public IsVisible(IValue<Transform> from, IValue<Collider> target, LayerMask mask)
        {
            this.from = from;
            this.target = target;

            this.mask = mask;
        }
        
        private IValue<Transform> from;
        private IValue<Collider> target;
        
        private LayerMask mask;
        
        public override bool Check(Packet packet)
        {
            if (!from.IsValid(packet) || !target.IsValid(packet)) return false;
            
            var corners = target.Value.bounds.GetCorners();
            return corners.Any(IsVisible);
                
            bool IsVisible(Vector3 point)
            {
                var direction = point - @from.Value.position;
                var ray = new Ray(@from.Value.position, direction);

                return !Physics.Raycast(ray, direction.magnitude, mask);
            }
        }
    }
}