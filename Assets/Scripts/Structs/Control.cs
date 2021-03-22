using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct Control
    {
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float limit;
        [SerializeField] private float factor;

        public float Process(float delta)
        {
            var value = Mathf.Clamp(Mathf.Abs(delta), 0, limit);
            return map.Evaluate(value / limit) * factor * Mathf.Sign(delta);
        }
    }

    [Serializable]
    public struct SmoothControl
    {
        [SerializeField] private float smoothing;
        [SerializeField] private Control control;

        private float current;
        private float velocity;

        public float Process(float delta)
        {
            var target = control.Process(delta);
            current = Mathf.SmoothDamp(current, target, ref velocity, smoothing);

            return current;
        }
    }
}