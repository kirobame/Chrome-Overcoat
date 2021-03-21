using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct LookControl
    {
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float limit;
        [SerializeField] private float speed;

        public float Impact(float delta)
        {
            var value = Mathf.Clamp(Mathf.Abs(delta), 0, limit);
            return map.Evaluate(value / limit) * speed * Mathf.Sign(delta);
        }
    }
}