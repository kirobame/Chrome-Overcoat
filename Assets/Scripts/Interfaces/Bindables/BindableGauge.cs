using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class BindableGauge : Bindable<float>
    {
        public event Action<float> onReachedMin;
        public event Action<float> onReachedMax;
        
        public BindableGauge(HUDBinding binding, Vector2 range) : base(binding) => this.range = range;
        public BindableGauge(HUDBinding binding, float initialValue, Vector2 range) : base(binding, initialValue) => this.range = range;

        public bool IsAtMin => value == range.x;
        public bool IsAtMax => value == range.y;
        public Vector2 Range => range; 
        
        [SerializeField] private Vector2 range;
        
        public float ComputeRatio() => Mathf.InverseLerp(range.x, range.y, value);

        protected override float HandleAssignment(float value)
        {
            if (value <= range.x)
            {
                if (this.value > range.x) onReachedMin?.Invoke(range.x);
                return range.x;
            }
            
            if (value >= range.y)
            {
                if (this.value < range.y) onReachedMax?.Invoke( range.y);
                return range.y;
            }

            return value;
        }
    }
}