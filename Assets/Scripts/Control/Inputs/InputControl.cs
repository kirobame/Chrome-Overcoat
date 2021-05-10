using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public abstract class InputControl<T> : MonoBehaviour, IInstaller, ILifebound, IInjectable, IInjectionCallbackListener where T : MonoBehaviour
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            OnInjectionDone(source);
            SetupInputs();
        }

        protected virtual void OnInjectionDone(IRoot source) { }
        protected virtual void SetupInputs() { }

        //--------------------------------------------------------------------------------------------------------------/
        
        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;

        protected IValue<InputHandler> input;

        protected virtual void Awake()
        {
            injections = new List<IValue>();
            
            input = new AnyValue<InputHandler>();
            injections.Add(input);
        }
        protected virtual void OnDestroy() => onDestruction?.Invoke(this);
        
        public virtual void Bootup()
        {
            input.Value.SetActiveAll(this, true);
            enabled = true;
        }
        public virtual void Shutdown()
        {
            input.Value.SetActiveAll(this, false);
            enabled = false;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set<T>(this as T);
    }
}