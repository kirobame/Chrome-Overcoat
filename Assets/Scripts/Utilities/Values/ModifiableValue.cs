using System;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class ModifiableValue<T> : IRegistry<T>, IBootable where T : struct
    {
        public object RawValue => current;
        public T Value => current;
        
        [SerializeField] private T value;
        private T current;
        
        private List<IModification<T>> modifications = new List<IModification<T>>();

        public void Bootup() => current = value;
        
        public void Update()
        {
            for (var i = 0; i < modifications.Count; i++)
            {
                if (!modifications[i].Update(value, current, out current)) continue;
                
                modifications.RemoveAt(i);
                i--;
            }
        }

        public void Set(object value) => current = (T)value;
        public void Modify(IModification<T> modification) => modifications.Add(modification);
    }
}