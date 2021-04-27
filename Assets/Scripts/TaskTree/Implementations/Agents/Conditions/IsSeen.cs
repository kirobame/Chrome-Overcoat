using UnityEngine;

namespace Chrome
{
    public class IsSeen : ConditionalNode
    {
        public IsSeen(IValue<Collider> target, IValue<LineOfSight> lineOfSight, IValue<Transform> aimTr)
        {
            this.target = target;
            this.aimTr = aimTr;
            this.lineOfSight = lineOfSight;
        }

        private IValue<Collider> target;
        private IValue<Transform> aimTr;
        private IValue<LineOfSight> lineOfSight;

        protected override bool Check(Packet packet)
        {
            if (!lineOfSight.IsValid(packet) || !target.IsValid(packet) || !aimTr.IsValid(packet)) return false;

            var r1 = lineOfSight.Value.transform.rotation;
            var r2 = aimTr.Value.rotation;

            var dotProduct = Quaternion.Dot(r1, r2);
            //Debug.Log(dotProduct);

            if (dotProduct > 0.5f || dotProduct < -0.5f) return false;

            return lineOfSight.Value.CanSee(target.Value);
        }
    }
}