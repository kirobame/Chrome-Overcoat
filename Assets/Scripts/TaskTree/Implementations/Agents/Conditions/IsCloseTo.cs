using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class IsCloseTo : ConditionalNode
    {
        public IsCloseTo(IValue<Transform> transform, IValue<Transform> target, float distanceMin)
        {
            this.transform = transform;
            this.target = target;
            this.distanceMin = distanceMin;
        }

        private IValue<Transform> transform;
        private IValue<Transform> target;
        private float distanceMin;

        protected override bool Check(Packet packet)
        {
            if (!target.IsValid(packet) || !transform.IsValid(packet)) return false;

            var distance = Vector3.Distance(transform.Value.position, target.Value.position);

            Debug.Log("Distance from target : " + distance);

            if (distance < distanceMin)
                return true;
            else
                return false;
        }
    }
}
