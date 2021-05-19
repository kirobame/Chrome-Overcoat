using System;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chrome
{
    public abstract class Loot : MonoBehaviour, IPickable, ILifebound, IInjectable, IInstaller
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        void IInjectable.PrepareInjection()
        {
            rigidbody = new AnyValue<Rigidbody>();
            identity = new AnyValue<IIdentity>();
            lifetime = new AnyValue<Lifetime>();
            
            injections = new List<IValue>()
            {
                identity,
                rigidbody,
                lifetime
            };
            
            PrepareInjection();
        }
        protected virtual void PrepareInjection() { }
        
        //--------------------------------------------------------------------------------------------------------------/

        private event Action<ILifebound> onLifeboundDestruction;
        event Action<ILifebound> IActive<ILifebound>.onDestruction
        {
            add => onLifeboundDestruction += value;
            remove => onLifeboundDestruction -= value;
        }

        private event Action<IInteraction> onInteractionDestruction;
        event Action<IInteraction> IActive<IInteraction>.onDestruction
        {
            add => onInteractionDestruction += value;
            remove => onInteractionDestruction -= value;
        }

        public Transform Transform => transform;
        public bool IsActive => true;

        [FoldoutGroup("Values"), SerializeField] private Vector2 angleRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 forceRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 offsetRange;

        protected new IValue<Rigidbody> rigidbody;
        protected IValue<Lifetime> lifetime;
        protected IValue<IIdentity> identity;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected virtual void OnDestroy()
        {
            onLifeboundDestruction?.Invoke(this);
            onInteractionDestruction?.Invoke(this);
        }

        public virtual void Bootup()
        {
            Repository.AddTo(Reference.Pickups, (IPickable)this);
            
            var direction = Quaternion.Euler(Random.Range(angleRange.x, angleRange.y), Random.Range(0.0f, 360f), 0.0f) * Vector3.forward;
            var point = rigidbody.Value.worldCenterOfMass + Random.insideUnitSphere * Random.Range(offsetRange.x, offsetRange.y);
            
            rigidbody.Value.AddForceAtPosition(direction * Random.Range(forceRange.x, forceRange.y), point, ForceMode.Impulse);
        }
        public virtual void Shutdown() => Repository.RemoveFrom(Reference.Pickups, (IPickable)this);

        public virtual void OnHoverStart(IIdentity source) { }
        public virtual void OnHoverEnd(IIdentity source) { }
        
        public void Pickup(IIdentity source)
        {
            OnPickup(source);
            lifetime.Value.End();
        }
        protected abstract void OnPickup(IIdentity source);
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}