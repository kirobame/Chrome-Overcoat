using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class ThrusterHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Jetpack;

        [SerializeField] private Image gauge;
        [SerializeField] private Gradient gradient;

        private BindableGauge gaugeBinding;

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);
            
            gaugeBinding = bindables.Get<BindableGauge>(HUDBinding.Gauge);
            gaugeBinding.onChange += OnGaugeChange;
        }
        public override void UnbindFromCurrent() => gaugeBinding.onChange -= OnGaugeChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnGaugeChange(float value)
        {
            var ratio = gaugeBinding.ComputeRatio();
            
            gauge.color = gradient.Evaluate(ratio);
            gauge.fillAmount = ratio / 2.0f;
        }
    }
}