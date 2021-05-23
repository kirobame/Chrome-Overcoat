using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class BindableCappedGauge : BindableGauge
    {
        public BindableCappedGauge(HUDBinding binding, Vector2 range, float cap) : base(binding, range) => this.cap = cap;
        public BindableCappedGauge(HUDBinding binding, float initialValue, Vector2 range, float cap) : base(binding, initialValue, range) => this.cap = cap;

        public bool IsUsable => value > cap;
        public bool IsLocked { get; set; }
        public float Cap => cap;

        [SerializeField] private float cap;
    }
}