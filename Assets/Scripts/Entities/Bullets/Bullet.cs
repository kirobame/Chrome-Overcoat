using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public abstract class Bullet : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        protected IIdentity identity;
        
        public Vector3 Advance => transform.position + direction * radius;
        
        [FoldoutGroup("Values"), SerializeField] protected float radius;
        [FoldoutGroup("Values"), SerializeField] protected float speed;
        [FoldoutGroup("Values"), SerializeField] protected LayerMask hitMask;

        protected float actualSpeed;
        protected Vector3 direction = Vector3.forward;

        private HashSet<Collider> ignores = new HashSet<Collider>();

        void Awake() => actualSpeed = speed;
        
        public virtual void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            ignores.Clear();
            
            transform.position = fireAnchor;
            this.direction = direction;
        }

        public void Ignore(Collider collider) => ignores.Add(collider);

        protected virtual void Update()
        {
            var length = actualSpeed * Time.deltaTime;
            var ray = new Ray(transform.position + direction * radius, direction);

            if (Physics.Raycast(ray, out var hit, length, hitMask) && !ignores.Contains(hit.collider)) OnHit(hit);
            else
            {
                var delta = direction * length;

                transform.position += delta;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        protected abstract void OnHit(RaycastHit hit);
    }
}