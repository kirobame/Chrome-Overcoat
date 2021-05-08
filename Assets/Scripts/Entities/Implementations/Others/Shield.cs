using System;
using System.Collections.Generic;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Shield : MonoBehaviour, IDamageable, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        void IInjectable.OnInjectionDone(IRoot source) { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        private event Action<IInteraction> onInteractionDestruction;
        event Action<IInteraction> IActive<IInteraction>.onDestruction
        {
            add => onInteractionDestruction += value;
            remove => onInteractionDestruction -= value;
        }

        private event Action<ILifebound> onLifeboundDestruction;
        event Action<ILifebound> IActive<ILifebound>.onDestruction
        {
            add => onLifeboundDestruction += value;
            remove => onLifeboundDestruction -= value;
        }

        public IIdentity Identity => identity.Value;
        public bool IsActive => true;
        
        [BoxGroup("Dependencies"), SerializeField] private Lifetime link;
        
        [FoldoutGroup("References"), SerializeField] private MeshRenderer mesh;
        [FoldoutGroup("References"), SerializeField] private MeshCollider col;

        [FoldoutGroup("Values"), SerializeField] private float maxHealth;

        private IValue<IIdentity> identity;
        private float health;

        public bool IsBroken => isBroken;
        private bool isBroken = false;

        void Awake()
        {
            identity = new AnyValue<IIdentity>();  
            injections = new IValue[] { identity };
            
            Bootup();
        }
        void OnDestroy()
        {
            onInteractionDestruction?.Invoke(this);
            onLifeboundDestruction?.Invoke(this);
        }

        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var difference = health - amount;
            var damage = difference < 0 ? amount + difference : amount;
            health -= damage;

            var type = identity.Value.Packet.Get<byte>();

            Events.ZipCall<byte, float>(GaugeEvent.OnDamageInflicted, type, damage);

            if (health <= 0) link.End();
        }

        public void Up()
        {
            if (!mesh.enabled)
                mesh.enabled = true;
            if (!col.enabled)
                col.enabled = true;
        }
        public void Down()
        {
            if (mesh.enabled)
                mesh.enabled = false;
            if (col.enabled)
                col.enabled = false;
        }

        public void Bootup()
        {
            //Debug.Log("Shield bootup");
            health = maxHealth;
            isBroken = false;
        }

        public void Shutdown()
        {
            isBroken = true;
        }
    }
}
