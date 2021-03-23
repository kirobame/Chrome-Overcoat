using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class ClampedViewRotation : ViewRotation
    {
        [SerializeField] private Vector2 range;

        protected override float ComputeRotation(float delta)
        {
            var rotation = base.ComputeRotation(delta);
            current = Mathf.Clamp(rotation, range.x, range.y);
            
            return current;
        }
    }
}