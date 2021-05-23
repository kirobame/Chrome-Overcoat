using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Bindable<T> : IBindable<T>
    {
        public Bindable(HUDBinding binding)
        {
            this.binding = binding;
            value = default;
        }
        public Bindable(HUDBinding binding, T initialValue)
        {
            this. binding = binding;
            value = initialValue;
        }
        
        public event Action<T> onChange;

        public HUDBinding Binding => binding;
        public T Value
        {
            get => value;
            set
            {
                this.value = HandleAssignment(value);
                onChange?.Invoke(value);
            }
        }

        [SerializeField] private HUDBinding binding;
        [SerializeField] protected T value;

        protected virtual T HandleAssignment(T value) => value;
    }
}