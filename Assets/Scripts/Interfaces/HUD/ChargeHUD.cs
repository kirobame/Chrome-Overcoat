using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Chrome
{
    public class ChargeHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Weapon;
        
        [SerializeField] private RectTransform threshold;
        [SerializeField] private Image charge;
        [SerializeField] private Gradient gradient;

        private BindableCappedGauge chargeBinding;

        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            chargeBinding = bindables.Get<BindableCappedGauge>(HUDBinding.Gauge);
            chargeBinding.onChange += OnChargeChange;

            var ratio = Mathf.InverseLerp(chargeBinding.Range.x, chargeBinding.Range.y, chargeBinding.Cap);
            threshold.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - ratio * 180.0f);
        }
        public override void UnbindFromCurrent() =>  chargeBinding.onChange -= OnChargeChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnChargeChange(float value)
        {
            var ratio = chargeBinding.ComputeRatio();

            charge.color = gradient.Evaluate(ratio);
            charge.fillAmount = ratio / 2.0f;
        }
    }
}
