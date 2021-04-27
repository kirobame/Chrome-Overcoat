using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome.Retro
{
    public class RetGauge : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private EventHandler handler;
        [FoldoutGroup("Dependencies"), SerializeField] private Slider slider;
        [FoldoutGroup("Dependencies"), SerializeField] private Image fill;

        [FoldoutGroup("Values"), SerializeField] private AnimationCurve color;
        [FoldoutGroup("Values"), SerializeField] private Color left;
        [FoldoutGroup("Values"), SerializeField] private Color right;
        [FoldoutGroup("Values"), SerializeField] private float max;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve map;
        [FoldoutGroup("Values"), SerializeField] private float rate;

        private float value;
        
        void Awake()
        {
            value = 0.25f * max;
            slider.value = value / max * slider.maxValue;
            
            handler.AddDependency(Events.Subscribe<byte, float>(GaugeEvent.OnDamageInflicted, OnDamageInflicted));
        }

        void Update()
        {
            value += map.Evaluate(value / max) * rate * Time.deltaTime;
            value = Mathf.Clamp(value, 0.0f, max);

            slider.value = value / max * slider.maxValue;
            fill.color = Color.Lerp(left, right, color.Evaluate(value / max));
        }

        void OnDamageInflicted(byte type, float amount) => value += 1.5f;
    }
}