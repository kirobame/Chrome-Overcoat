using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class ShakeControl : BaseShakeControl
    {
        [FoldoutGroup("Effect"), SerializeField]
        private float smoothing;

        [FoldoutGroup("Effect"), SerializeField]
        private Vector2 lengthRange;

        [FoldoutGroup("Amplitude"), SerializeField]
        private AnimationCurve amplitudeMap;

        [FoldoutGroup("Amplitude"), SerializeField]
        private float maxAmplitude;

        [FoldoutGroup("Frequency"), SerializeField]
        private AnimationCurve frequencyMap;

        [FoldoutGroup("Frequency"), SerializeField]
        private float maxFrequency;

        private Vector3 anchor;
        private Vector3 value;

        private Vector3 settle;
        private Vector3 zeroed;

        private float timer;

        protected override void Awake()
        {
            anchor = transform.localPosition;
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            transform.localPosition =
                Vector3.SmoothDamp(transform.localPosition, anchor + value, ref settle, smoothing);
            value = Vector3.SmoothDamp(value, Vector3.zero, ref zeroed, reduction);
        }

        protected override void Settle()
        {
            timer = 0.0f;
            value = Vector3.zero;
        }

        protected override void Compute()
        {
            var ratio = Intensity / Range;
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
    }
}