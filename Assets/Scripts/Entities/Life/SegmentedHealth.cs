using UnityEngine;

namespace Chrome
{
    public class SegmentedHealth : Health
    {
        protected override BindableGauge BuildGauge() => new SegmentedBindableGauge(HUDBinding.Gauge, Max, new Vector2(0.0f, Max));

        protected override float ProcessDamage(float amount) => Mathf.RoundToInt(amount);
    }
}