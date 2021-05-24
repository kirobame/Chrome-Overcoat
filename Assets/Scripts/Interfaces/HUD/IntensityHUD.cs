using TMPro;
using UnityEngine;

namespace Chrome
{
    public class IntensityHUD : HUD
    {
        public override HUDGroup WantedGroups => HUDGroup.Intensity;

        [SerializeField] private RectTransform fillUnder50;
        [SerializeField] private RectTransform fillAbove50;
        [SerializeField] private TMP_Text percentage;

        private BindableGauge intensityBinding;
        
        public override void BindTo(RectTransform frame, IBindable[] bindables)
        {
            RectTransform.SetParent(frame, false);

            intensityBinding = bindables.Get<BindableGauge>(HUDBinding.Gauge);
            intensityBinding.onChange += OnIntensityChange;
            
            OnIntensityChange(intensityBinding.Value);
        }
        public override void UnbindFromCurrent() => intensityBinding.onChange -= OnIntensityChange;

        //--------------------------------------------------------------------------------------------------------------/

        void OnIntensityChange(float value)
        {
            var ratio = intensityBinding.ComputeRatio();
            if (ratio <= 0.5f)
            {
                fillUnder50.localScale = new Vector3(ratio * 2.0f, 1.0f, 1.0f);
                fillAbove50.localScale = new Vector3(0.0f, 1.0f, 1.0f);
            }
            else
            {
                fillUnder50.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                fillAbove50.localScale = new Vector3((ratio - 0.5f) * 2.0f, 1.0f, 1.0f);
            }

            var roundedRatio = Mathf.FloorToInt(ratio * 100.0f);
            if (roundedRatio >= 10) percentage.text = $"{roundedRatio.ToString()}%";
            else percentage.text = $"0{roundedRatio.ToString()}%";
        }
    }
}