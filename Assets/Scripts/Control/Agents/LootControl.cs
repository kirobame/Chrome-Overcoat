using System;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class LootControl : MonoBehaviour, ILifebound, IAssignable<Agent>
    {
        public event Action<ILifebound> onDestruction;

        public object Value => Owner;
        public Agent Owner { get; private set; }

        public bool IsActive => true;

        void OnDestroy() => onDestruction?.Invoke(this);

        //--------------------------------------------------------------------------------------------------------------/

        public void AssignTo(Agent owner) => Owner = owner;
        
        public void Bootup() { }
        public void Shutdown()
        {
            Debug.Log($"[{transform.root.gameObject.name}] DROPPING LOOT !");
            Routines.Start(Routines.DoAfter(() => Owner.Lifetime.RemoveBound(this), new YieldFrame()));
        }
    }
}