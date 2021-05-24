using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class IsCloseTo : Condition
    {
        public IsCloseTo(float range, IValue<Transform> transform, IValue<Transform> target)
        {
            this.range = range;
            
            this.transform = transform;
            this.target = target;
        }

        private float range;
        
        private IValue<Transform> transform;
        private IValue<Transform> target;
        
        public override bool Check(Packet packet)
        {
            if (!target.IsValid(packet) || !transform.IsValid(packet)) return false;

            var distance = Vector3.Distance(transform.Value.position, target.Value.position);
            return distance < range;
        }
    }
}
