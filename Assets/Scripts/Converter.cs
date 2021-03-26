using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Converter
    {
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float midPoint;
        [SerializeField] private Vector2 range;

        public float Process(float ratio)
        {
            var interpolation = map.Evaluate(ratio);
            
            if (interpolation < 0) return Mathf.Lerp(range.x, midPoint, interpolation + 1.0f);
            else return Mathf.Lerp(midPoint, range.y, interpolation);
        }
    }
}