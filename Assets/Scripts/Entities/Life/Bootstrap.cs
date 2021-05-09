using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class Bootstrap : MonoBehaviour, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            hasBeenBootedUp = true;
            Routines.Start(Routines.DoAfter(OnEnable, new YieldFrame()));
        }

        //--------------------------------------------------------------------------------------------------------------/

        private bool hasBeenBootedUp;
        private IValue<Lifetime> lifetime;

        void Awake()
        {
            hasBeenBootedUp = false;
            
            lifetime = new AnyValue<Lifetime>();
            injections = new IValue[] { lifetime};
        }
        void OnEnable()
        {
            if (!hasBeenBootedUp) return;
            lifetime.Value.Begin();
        }
    }
}