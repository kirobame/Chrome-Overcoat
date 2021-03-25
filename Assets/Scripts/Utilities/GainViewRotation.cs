using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class GainViewRotation : ViewRotation
    {
        [Space, SerializeField] private float threshold;
        [SerializeField] private AnimationCurve gainMap;
        [SerializeField] private float gainFactor;
        [SerializeField] private float gainTime;

        private float time = -1.0f;

        protected override float ComputeRotation(float delta)
        {
            var gain = 0.0f;
            if (Mathf.Abs(delta) >= threshold)
            {
                if (time <= 0.0f) time = Time.deltaTime;
                else time += Time.deltaTime;

                gain = gainMap.Evaluate(Mathf.Clamp(time, 0, gainTime) / gainTime);
                gain *= gainFactor * Mathf.Sign(delta);
            }
            else time = -1.0f;
            
            return base.ComputeRotation(delta + gain);
        }
    }
}