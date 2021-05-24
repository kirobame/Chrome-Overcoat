using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class HealthHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Health;

        [SerializeField] private Slider health;
        [SerializeField] private Slider roundedHealth;
        [SerializeField] private RectTransform[] bars;
        
        private SegmentedBindableGauge healthBinding;
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            healthBinding = bindables.Get<SegmentedBindableGauge>(HUDBinding.Gauge);
            healthBinding.onChange += OnHealthChange;

            var count = (int)healthBinding.Range.y - 1;
            for (var i = 0; i < count; i++) bars[i].gameObject.SetActive(true);
            for (var i = count; i <  bars.Length; i++) bars[i].gameObject.SetActive(false);
        }
        public override void UnbindFromCurrent() => healthBinding.onChange -= OnHealthChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnHealthChange(float value)
        {
            health.value = healthBinding.ComputeRatio();
            roundedHealth.value = healthBinding.RoundedValue / healthBinding.Range.y;
        }
    }
}