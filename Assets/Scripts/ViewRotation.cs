using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class ViewRotation : IMouseHandler
    {
        public bool IsLocked { get; private set; }
        
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float limit;
        [SerializeField] private float factor;
        [SerializeField] private float smoothing;

        protected float current;
        private float velocity;

        public void Lock() => IsLocked = true;
        public void Unlock() => IsLocked = false;

        public void Bootup(float rotation) => current = rotation;
        public float Process(float delta)
        {
            if (IsLocked) return current;

            return ComputeRotation(delta);
        }

        protected virtual float ComputeRotation(float delta)
        {
            var target = Mathf.Clamp(Mathf.Abs(delta), 0, limit);
            target = map.Evaluate(target / limit) * factor * Mathf.Sign(delta);

            current = Mathf.SmoothDamp(current, current + target, ref velocity, smoothing);
            return current;
        }
    }
}