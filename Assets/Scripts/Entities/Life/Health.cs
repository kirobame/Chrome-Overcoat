using System;
using System.Collections.Generic;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Health : MonoBehaviour, IDamageable, IHealable, ILifebound, IInstaller, IInjectable
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
        public float Amount => gauge.Value;

        [FoldoutGroup("Values"), SerializeField] private float max;

        private IValue<IIdentity> identity;
        private IValue<Lifetime> lifetime;

        private BindableGauge gauge;

        //--------------------------------------------------------------------------------------------------------------/

        void OnDestroy()
        {
            onInteractionDestruction?.Invoke(this);
            onLifeboundDestruction?.Invoke(this);
        }

        public void Bootup(byte code)
        {
            gauge = BuildGauge();

            var board = identity.Value.Packet.Get<IBlackboard>();
            if (board.TryGet<byte>(Refs.TYPE, out var type) && type == PlayerRefs.TYPE_VALUE) HUDBinder.Declare(HUDGroup.Health, gauge);
            
            gauge.Value = gauge.Range.y;
        }
        protected virtual BindableGauge BuildGauge() => new BindableGauge(HUDBinding.Gauge, max, new Vector2(0.0f, max));

        public void Shutdown(byte code) { }

        //--------------------------------------------------------------------------------------------------------------/
        
        public virtual void Hit(IIdentity source, float amount, Packet packet)
        {
            if (gauge.IsAtMin) return;
            
            var difference = gauge.Value - amount;
            var damage = difference < 0 ? amount + difference : amount;

            gauge.Value -= ProcessDamage(damage);
            onChange?.Invoke(gauge.Value, gauge.Range.y);

            if (gauge.IsAtMin) lifetime.Value.End();
        }
        protected virtual float ProcessDamage(float amount) => amount;
        
        public virtual void Heal(IIdentity source, float amount, Packet packet)
        {
            if (gauge.IsAtMax) return;

            var difference = gauge.Range.y - gauge.Value;
            var heal = amount > difference ? difference : amount;
            
            gauge.Value += ProcessHeal(heal);
            onChange?.Invoke(gauge.Value, gauge.Range.y);
        }
        protected virtual float ProcessHeal(float amount) => amount;

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}