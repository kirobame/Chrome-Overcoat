using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public abstract class BaseShakeControl : MonoBehaviour
    {
        public float Intensity => intensity;
        public float Range => range;
        
        [FoldoutGroup("Values"), SerializeField] protected float reduction;
        [FoldoutGroup("Values"), SerializeField] protected float range;

        private float intensity;
        private float velocity;
        
        protected virtual void Awake()
        {
            intensity = 0.0f;
            velocity = 0.0f;

            Events.Subscribe<float,float>(PlayerEvent.OnShake, Add);
        }
        void OnDestroy() => Events.Unsubscribe<float,float>(PlayerEvent.OnShake, Add);
        
        protected virtual void Update()
        {
            if (intensity <= Mathf.Epsilon) Settle();
            else Compute();
            
            intensity = Mathf.SmoothDamp(intensity, 0.0f, ref velocity, reduction);
        }

        public void Add(float value, float max)
        {
            var addition = intensity + value;
            if (addition > max)
            {
                value -= addition - max;
                if (value < 0.0f) value = 0.0f;
            }
            
            intensity += value;
            if (intensity > range) intensity = range;
        }

        protected abstract void Settle();
        protected abstract void Compute();
    }
}