using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Acceleration
    {
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float time;
        [SerializeField] private float threshold;

        private int previousSign;
        private float timer;
        
        public float Process(float delta)
        {
            var sign = Mathf.RoundToInt(Mathf.Sign(delta));
            
            if (previousSign == sign && Mathf.Abs(delta) > threshold)
            {
                timer += Time.deltaTime;
                if (timer > time) timer = time;
            }
            else timer = 0.0f;

            previousSign = sign;
            return map.Evaluate(timer / time);
        }
    }
}