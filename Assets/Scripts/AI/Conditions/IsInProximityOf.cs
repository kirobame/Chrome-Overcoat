using Chrome.Retro;
using UnityEngine;

namespace Chrome
{
    public class IsInProximityOf : Condition
    {
        public IsInProximityOf(IValue<Transform> from, IValue<Transform> target, float distance)
        {
            this.from = from;
            this.target = target;

            this.distance = distance;
        }

        private IValue<Transform> from;
        private IValue<Transform> target;

        private float distance;
        
        public override bool Check(Packet packet)
        {
            if (!from.IsValid(packet) || !target.IsValid(packet)) return false;
            return Vector3.Distance(from.Value.position.Flatten(), target.Value.position.Flatten()) < distance;
        }
    }
}