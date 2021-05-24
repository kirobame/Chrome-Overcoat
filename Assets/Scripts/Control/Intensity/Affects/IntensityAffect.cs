using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public abstract class IntensityAffect : MonoBehaviour, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        void IInjectable.PrepareInjection()
        {
            intensity = new AnyValue<IntensityControl>();
            injections = new List<IValue>() { intensity };

            PrepareInjection();
        }
        protected virtual void PrepareInjection() { }

        //--------------------------------------------------------------------------------------------------------------/

        public event Action<ILifebound> onDestruction;
        
        public IntensityControl Intensity => intensity.Value;
        private IValue<IntensityControl> intensity;
        
        public bool IsActive => true;

        protected virtual void Start() => enabled = false;
        protected virtual void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup(byte code) => enabled = true;
        public void Shutdown(byte code) => enabled = false;
    }
}