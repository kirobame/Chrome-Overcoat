﻿using System;
using System.Collections.Generic;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Health : MonoBehaviour, IDamageable, ILifebound, IInstaller, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            identity = new AnyValue<IIdentity>();
            lifetime = new AnyValue<Lifetime>();
            
            injections = new IValue[]
            {
                identity,
                lifetime
            };
        }

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

        public float Max => max;
        public float Amount => amount;

        [FoldoutGroup("Values"), SerializeField] private float max;

        private IValue<IIdentity> identity;
        private IValue<Lifetime> lifetime;
        
        private float amount;

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnDestroy()
        {
            onInteractionDestruction?.Invoke(this);
            onLifeboundDestruction?.Invoke(this);
        }

        public void Bootup() => amount = max;

        public void Shutdown() { }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Hit(IIdentity source, float amount, Packet packet)
        {
            if (this.amount == 0) return;
            
            var difference = this.amount - amount;
            var damage = difference < 0 ? amount + difference : amount;
            
            this.amount = Mathf.Clamp(this.amount - damage, 0.0f, max);
            onChange?.Invoke(this.amount, max);

            var sourceType = source.Packet.Get<byte>();
            if (identity.Value.Faction == Faction.Player) Events.ZipCall<float,byte>(GaugeEvent.OnDamageReceived, damage, sourceType);
            else if (source.Faction == Faction.Player)
            {
                var type = identity.Value.Packet.Get<byte>();
                
                Events.ZipCall<byte,float,byte>(GaugeEvent.OnDamageInflicted, type, damage, sourceType);
                if (this.amount == 0) Events.ZipCall<byte,byte>(GaugeEvent.OnKill, type, sourceType);
            }
            
            if (this.amount == 0) lifetime.Value.End();
        }

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}