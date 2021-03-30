using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Gain
    {
        public float Timer { get; private set; }
        
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float time;

        public void SetTimer(float ratio) => Timer = time * Mathf.Clamp01(ratio);
        
        public float Compute()
        {
            Timer += Time.deltaTime;
            Timer = Mathf.Clamp(Timer, 0, time);

            return map.Evaluate(Timer / time);
        }
    }
}