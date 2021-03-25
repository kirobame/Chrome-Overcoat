using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class ViewRotation : IViewAxisHandler
    {
        public bool IsLocked { get; private set; }
        
        [SerializeField] protected AnimationCurve map;
        [SerializeField] protected float limit;
        [SerializeField] protected float factor;
        [SerializeField] protected float smoothing;

        protected float current;
        protected float velocity;

        public void Lock() => IsLocked = true;
        public void Unlock() => IsLocked = false;

        public void Set(float rotation) => current = rotation;
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