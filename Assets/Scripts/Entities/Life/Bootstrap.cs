using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class Bootstrap : MonoBehaviour, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/

        private IValue<Lifetime> lifetime;

        void Awake()
        {
            lifetime = new AnyValue<Lifetime>();
            injections = new IValue[] { lifetime};
        }
        void OnEnable() => lifetime.Value.Begin();
    }
}