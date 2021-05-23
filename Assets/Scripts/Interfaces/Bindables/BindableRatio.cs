using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class BindableRatio : Bindable<float>
    {
        public BindableRatio(HUDBinding binding, Vector2 range) : base(binding) => this.range = range;
        public BindableRatio(HUDBinding binding, float initialValue, Vector2 range) : base(binding, initialValue) => this.range = range;

        public bool IsAtMin => Compute() <= range.x;
        public bool IsAtMax => Compute() >= range.y;
        public Vector2 Range => range; 
        
        [SerializeField] private Vector2 range;

        public float Compute() => Mathf.InverseLerp(range.x, range.y, value);
    }
}