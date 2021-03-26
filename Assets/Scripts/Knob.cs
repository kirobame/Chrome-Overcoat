using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Knob
    {
        public float Value => current;
        public float Smoothing
        {
            get => smoothing;
            set => smoothing = value;
        }
        
        [SerializeField] protected AnimationCurve map;
        [SerializeField] protected float limit;
        [SerializeField] protected float factor;
        [SerializeField] protected float smoothing;

        protected float current;
        protected float velocity;

        public void Set(float value) => current = value;

        public float Process(float delta)
        {
            var target = Mathf.Clamp(Mathf.Abs(delta), 0.0f, limit);
            target = map.Evaluate(target / limit) * factor * Mathf.Sign(delta);

            current = Mathf.SmoothDamp(current, target, ref velocity, smoothing);
            return current;
        }
    }
}