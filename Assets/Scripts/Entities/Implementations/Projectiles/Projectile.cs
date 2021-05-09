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

        void IInjectionCallbackListener.OnInjectionDone(IRoot source) => hasBeenBootedUp = true;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected Vector3 direction = Vector3.forward;
        protected HashSet<Collider> ignores = new HashSet<Collider>();

        protected Packet packet => identity.Value.Packet;
        protected IValue<IIdentity> identity;

        private bool hasBeenBootedUp;
        
        //--------------------------------------------------------------------------------------------------------------/

        protected virtual void Awake()
        {
            hasBeenBootedUp = false;
            
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
        }
        protected virtual void Bootup() { }
        
        public void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            ignores.Clear();
            
            transform.position = fireAnchor;
            this.direction = direction;

            if (!hasBeenBootedUp) Routines.Start(BootupRoutine(source, fireAnchor, direction, packet));
            else OnShoot(source, fireAnchor, direction, packet);
        }
        protected virtual void OnShoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet) => identity.Value.Copy(source);

        public void Ignore(Collider collider) => ignores.Add(collider);

        private IEnumerator BootupRoutine(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            while (!hasBeenBootedUp) yield return new WaitForEndOfFrame();

            Bootup();
            OnShoot(source, fireAnchor, direction, packet);
        }
    }
}