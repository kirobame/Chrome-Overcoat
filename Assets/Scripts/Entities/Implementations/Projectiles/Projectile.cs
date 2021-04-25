using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public abstract class Projectile : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        protected IIdentity identity;
        
        protected Vector3 direction = Vector3.forward;
        protected HashSet<Collider> ignores = new HashSet<Collider>();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public virtual void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            ignores.Clear();
            
            transform.position = fireAnchor;
            this.direction = direction;
        }

        public void Ignore(Collider collider) => ignores.Add(collider);
    }
}