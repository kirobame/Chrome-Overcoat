using System;
using System.Collections;
using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    public abstract class Projectile : MonoBehaviour, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
        }

        void IInjectionCallbackListener.OnInjectionDone(IRoot source) => Bootup();

        //--------------------------------------------------------------------------------------------------------------/
        
        protected Vector3 direction = Vector3.forward;
        protected HashSet<Collider> ignores = new HashSet<Collider>();

        protected Packet packet => identity.Value.Packet;
        protected IValue<IIdentity> identity;


        //--------------------------------------------------------------------------------------------------------------/
        
        protected virtual void Bootup() { }
        
        public void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            ignores.Clear();
            
            transform.position = fireAnchor;
            this.direction = direction;

            OnShoot(source, fireAnchor, direction, packet);
        }
        protected virtual void OnShoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet) => identity.Value.Copy(source);

        public void Ignore(Collider collider) => ignores.Add(collider);
    }
}