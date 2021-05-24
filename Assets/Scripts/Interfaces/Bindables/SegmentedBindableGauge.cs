using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class SegmentedBindableGauge : BindableGauge
    {
        public SegmentedBindableGauge(HUDBinding binding, Vector2 range) : base(binding, range)
        {
            range.x = Mathf.Floor(range.x);
            range.y = Mathf.Floor(range.y);
        }
        public SegmentedBindableGauge(HUDBinding binding, float initialValue, Vector2 range) : base(binding, initialValue, range)
        {
            range.x = Mathf.Floor(range.x);
            range.y = Mathf.Floor(range.y);
            
            RoundedValue = Mathf.RoundToInt(initialValue);
        }

        public override bool IsAtMin => RoundedValue == Mathf.FloorToInt(Range.x);
        public override bool IsAtMax => RoundedValue == Mathf.FloorToInt(Range.y);
        public int RoundedValue { get; private set; }

        protected override float HandleAssignment(float value)
        {
            var flooredValue = Mathf.FloorToInt(value);
            RoundedValue = flooredValue;

            return value;
        }
    }
}