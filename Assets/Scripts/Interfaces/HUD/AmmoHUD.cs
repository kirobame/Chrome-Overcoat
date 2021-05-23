using TMPro;
using UnityEngine;

namespace Chrome
{
    public class AmmoHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Weapon;

        [SerializeField] private TMP_Text ammo;

        private Bindable<float> ammoBinding;
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            ammoBinding = bindables.Get<Bindable<float>>(HUDBinding.Ammo);
            ammoBinding.onChange += OnAmmoChange;

            OnAmmoChange(ammoBinding.Value);
        }
        public override void UnbindFromCurrent() => ammoBinding.onChange -= OnAmmoChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnAmmoChange(float value)
        {
            if (value < 0.0f)
            {
                ammo.text = "000";
                return;
            }

            var roundedValue = Mathf.RoundToInt(value);
            if (roundedValue >= 100) ammo.text = roundedValue.ToString();
            else if (roundedValue >= 10) ammo.text = $"0{roundedValue}";
            else ammo.text = $"00{roundedValue}";
        }
    }
}