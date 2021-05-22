using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class JetpackHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Jetpack;
        
        [Space, SerializeField] private Image charge;

        [Space, SerializeField] private Image icon;
        [SerializeField] private TMP_Text cooldown;
        [SerializeField] private Image cooldownFill;

        private BindableRatio chargeBinding;
        private BindableCooldown cooldownBinding;
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            chargeBinding = bindables.Get<BindableRatio>(HUDBinding.Charge);
            chargeBinding.onChange += OnChargeChange;
            
            cooldownBinding = bindables.Get<BindableCooldown>(HUDBinding.Cooldown);
            cooldownBinding.onStart += OnCooldownStart;
            cooldownBinding.onChange += OnCooldownChange;
            cooldownBinding.onEnd += OnCooldownEnd;
        }
        public override void UnbindFromCurrent()
        {
            chargeBinding.onChange -= OnChargeChange;
            
            cooldownBinding.onStart -= OnCooldownStart;
            cooldownBinding.onChange -= OnCooldownChange;
            cooldownBinding.onEnd -= OnCooldownEnd;
        }

        void OnChargeChange(float value) => charge.fillAmount = chargeBinding.Compute();

        void OnCooldownStart(float value)
        {
            icon.enabled = false;
            cooldown.enabled = true;
        }
        void OnCooldownChange(float value)
        {
            var ratio = value / cooldownBinding.Time;
            cooldownFill.fillAmount = ratio;

            var remain = Mathf.CeilToInt(value);
            cooldown.text = remain < 10 ? $"0{remain}" : remain.ToString();
        }
        void OnCooldownEnd(float value)
        {
            icon.enabled = true;
            cooldown.enabled = false;
            
            cooldownFill.fillAmount = 0.0f;
        }
    }
}