using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class ShakeControl : MonoBehaviour
    {
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        [FoldoutGroup("Values"), SerializeField] private float reduction;
        [FoldoutGroup("Values"), SerializeField] private float range;
        [FoldoutGroup("Values"), SerializeField] private Vector2 lengthRange;
        
        [FoldoutGroup("Amplitude"), SerializeField] private AnimationCurve amplitudeMap;
        [FoldoutGroup("Amplitude"), SerializeField] private float maxAmplitude;
        
        [FoldoutGroup("Frequency"), SerializeField] private AnimationCurve frequencyMap;
        [FoldoutGroup("Frequency"), SerializeField] private float maxFrequency;

        private Vector3 anchor;
        private Vector3 value;
        private Vector3 settle;
        private Vector3 zeroed;

        private float intensity;
        private float velocity;
        
        private float timer;
        
        void Awake()
        {
            anchor = transform.localPosition;
            
            intensity = 0.0f;
            velocity = 0.0f;
            timer = 0.0f;

            Events.Subscribe<float,float>(PlayerEvent.OnShake, Add);
        }
        void OnDestroy() => Events.Unsubscribe<float,float>(PlayerEvent.OnShake, Add);
        
        void Update()
        {
            if (intensity <= Mathf.Epsilon)
            {
                timer = 0.0f;
                value = Vector3.zero;
            }
            else
            {
                var ratio = intensity / range;
                var step = frequencyMap.Evaluate(ratio) * maxFrequency;

                timer += Time.deltaTime;
                if (timer >= 1.0f / step)
                {
                    timer = 0.0f;
                    var amplitude = amplitudeMap.Evaluate(ratio) * maxAmplitude;

                    value = Random.insideUnitSphere * (lengthRange.y * amplitude);
                    value = value.normalized * Mathf.Lerp(lengthRange.x, lengthRange.y * amplitude, Random.value);
                }
            }
            
            intensity = Mathf.SmoothDamp(intensity, 0.0f, ref velocity, reduction);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, anchor + value, ref settle, smoothing);
            value = Vector3.SmoothDamp(value, Vector3.zero, ref zeroed, reduction);
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
    }
}