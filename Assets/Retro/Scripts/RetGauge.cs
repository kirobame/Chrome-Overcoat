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
            var playerBoard = Blackboard.Global.Get<RetPlayerBoard>(RetPlayerBoard.REF_SELF);
            if (!playerBoard.Identity.Root.gameObject.activeInHierarchy) return;
            
            value += map.Evaluate(value / max) * rate * Time.deltaTime;
            value = Mathf.Clamp(value, 0.0f, max);

            var ratio = value / max;
            slider.value = ratio * slider.maxValue;
            fill.color = Color.Lerp(left, right, color.Evaluate(ratio));

            if (ratio <= 0.0f || ratio >= 1.0f)
            {
                var life = playerBoard.Get<Lifetime>(RetPlayerBoard.REF_LIFE);
                life.End();
                
                value = 0.25f * max;
                slider.value = value / max * slider.maxValue;
            }
        }

        public void Modify(float amount) => value += amount;

        void OnDamageInflicted(byte type, float amount) => value += 1.5f;
    }
}