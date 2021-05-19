using System;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class LootControl : MonoBehaviour, ILifebound, IAssignable<Agent>
    {
        public event Action<ILifebound> onDestruction;

        public object Value => Owner;
        public Agent Owner { get; private set; }

        public bool IsActive => true;

        [FoldoutGroup("Values"), SerializeField] private GenericPoolable lootPrefab;
        [FoldoutGroup("Values"), SerializeField] private Vector3 offset;
        
        //--------------------------------------------------------------------------------------------------------------/

        void OnDestroy() => onDestruction?.Invoke(this);
        
        public void AssignTo(Agent owner) => Owner = owner;
        
        public void Bootup() { }
        public void Shutdown()
        {
            var lootInstance = lootPrefab.GetGenericInstance<Loot>(Pool.Loot);
            lootInstance.Transform.position = transform.position + offset;
            
            Routines.Start(Routines.DoAfter(() => Owner.Lifetime.RemoveBound(this), new YieldFrame()));
        }
    }
}