using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class LifetimeBootstrap : MonoBehaviour, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            lifetime = new AnyValue<Lifetime>();
            injections = new IValue[] { lifetime};
        }
        //--------------------------------------------------------------------------------------------------------------/

        private bool hasBeenBootedUp;
        private IValue<Lifetime> lifetime;

        void Awake() => hasBeenBootedUp = false;
        void Start()
        {
            hasBeenBootedUp = true;
            OnEnable();
        }
        
        void OnEnable()
        {
            if (!hasBeenBootedUp) return;
            lifetime.Value.Begin();
        }
    }
}