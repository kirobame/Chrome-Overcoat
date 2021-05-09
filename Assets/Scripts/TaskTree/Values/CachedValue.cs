using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CachedValue<T> : IValue<T>
    {
        public CachedValue(T value) => this.value = value;
        
        public object RawValue => value;
        public T Value
        {
            get => value;
            set => this.value = value;
        }

        [SerializeField] private T value;

        public void FillIn(Packet packet) { }
        public bool IsValid(Packet packet) => true;
    }
}