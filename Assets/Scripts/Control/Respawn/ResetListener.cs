using System;
using System.Collections.Generic;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class ResetListener : MonoBehaviour, IInjectable, ILifebound
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            lifetime = new AnyValue<Lifetime>();
            injections = new IValue[] { lifetime };
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public event Action<ILifebound> onDestruction;

        public bool IsActive => true;

        private IValue<Lifetime> lifetime;

        //--------------------------------------------------------------------------------------------------------------/

        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup(byte code) => Events.Subscribe(GlobalEvent.OnReset, OnReset);
        public void Shutdown(byte code) => Events.Unsubscribe(GlobalEvent.OnReset, OnReset);

        void OnReset() => lifetime.Value.End(99);
    }
}