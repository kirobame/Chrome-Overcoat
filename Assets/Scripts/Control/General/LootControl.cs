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
        
        //--------------------------------------------------------------------------------------------------------------/

        void OnDestroy() => onDestruction?.Invoke(this);
        
        public void AssignTo(Agent owner) => Owner = owner;
        
        public void Bootup(byte code) { }
        public void Shutdown(byte code)
        {
            if (code == 99) return;
            
            var lootInstance = lootPrefab.GetGenericInstance<Loot>(Pool.Loot);
            if (lootInstance is IRebootable rebootable) rebootable.Reboot();

            var ray = new Ray(transform.position + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out var hit, 100.0f, LayerMask.GetMask("Environment"))) lootInstance.transform.position = hit.point;
            else lootInstance.transform.position = transform.position;
            
            Routines.Start(Routines.DoAfter(() => Owner.Lifetime.RemoveBound(this), new YieldFrame()));
        }
    }
}