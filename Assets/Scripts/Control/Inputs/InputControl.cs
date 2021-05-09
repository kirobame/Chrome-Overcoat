using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public abstract class InputControl<T> : MonoBehaviour, IInstaller, ILifebound, IInjectable where T : MonoBehaviour
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        //--------------------------------------------------------------------------------------------------------------/
        
        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;

        protected IValue<InputHandler> input;

        protected virtual void Awake()
        {
            input = new AnyValue<InputHandler>();
            injections = new List<IValue>();
            injections.Add(input);
        }
        protected virtual void OnDestroy() => onDestruction?.Invoke(this);
        
        public virtual void Bootup() => enabled = true;
        public virtual void Shutdown() => enabled = false;

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set<T>(this as T);
    }
}