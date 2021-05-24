using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class StompHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Stomp;

        [Space, SerializeField] private Image icon;
        [SerializeField] private TMP_Text cooldown;
        [SerializeField] private Image cooldownFill;

        private BindableCooldown cooldownBinding;
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            cooldownBinding = bindables.Get<BindableCooldown>(HUDBinding.Cooldown);
            cooldownBinding.onStart += OnCooldownStart;
            cooldownBinding.onChange += OnCooldownChange;
            cooldownBinding.onEnd += OnCooldownEnd;
        }
        public override void UnbindFromCurrent()
        {
            cooldownBinding.onStart -= OnCooldownStart;
            cooldownBinding.onChange -= OnCooldownChange;
            cooldownBinding.onEnd -= OnCooldownEnd;
        }

        //--------------------------------------------------------------------------------------------------------------/

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