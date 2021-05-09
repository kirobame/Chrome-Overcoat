using System;
using System.Collections.Generic;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Health : MonoBehaviour, IDamageable, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
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
        
        public event Action<float, float> onChange;

        public IIdentity Identity => identity.Value;
        public bool IsActive => true;

        [FoldoutGroup("Values"), SerializeField] private float maxHealth;

        private IValue<IIdentity> identity;
        private IValue<Lifetime> lifetime;
        private float health;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            lifetime = new AnyValue<Lifetime>();
            injections = new IValue[]
            {
                identity,
                lifetime
            };
            
            Bootup();
        }
        void OnDestroy()
        {
            onInteractionDestruction?.Invoke(this);
            onLifeboundDestruction?.Invoke(this);
        }

        public void Bootup() => health = maxHealth;
        public void Shutdown() { }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var difference = health - amount;
            var damage = difference < 0 ? amount + difference : amount;
            
            health = Mathf.Clamp(health - damage, 0.0f, maxHealth);
            onChange?.Invoke(health, maxHealth);

            var sourceType = source.Packet.Get<byte>();
            if (identity.Value.Faction == Faction.Player) Events.ZipCall<float,byte>(GaugeEvent.OnDamageReceived, damage, sourceType);
            else if (source.Faction == Faction.Player)
            {
                var type = identity.Value.Packet.Get<byte>();
                
                Events.ZipCall<byte,float,byte>(GaugeEvent.OnDamageInflicted, type, damage, sourceType);
                if (health == 0) Events.ZipCall<byte,byte>(GaugeEvent.OnKill, type, sourceType);
            }
            
            if (health == 0) lifetime.Value.End();
        }
    }
}