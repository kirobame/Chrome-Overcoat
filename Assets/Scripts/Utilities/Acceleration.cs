using System;
using UnityEngine;

namespace Chrome
{
    public static class PhysicExtensions
    {
        public static Vector3[] GetCorners(this Bounds bounds)
        {
            return new Vector3[]
            {
                bounds.min,
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), 
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), 
                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), 
                
                bounds.max,
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), 
                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), 
                new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), 
            };
        }
    }
    
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