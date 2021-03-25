using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class SmoothedViewRotation : ViewRotation
    {
        protected override float ComputeRotation(float delta)
        {
            var target = Mathf.Clamp(Mathf.Abs(delta), 0, limit);
            target = map.Evaluate(target / limit) * factor * Mathf.Sign(delta);

            current = Mathf.SmoothDamp(current, target, ref velocity, smoothing);
            return current;
        }
    }
}