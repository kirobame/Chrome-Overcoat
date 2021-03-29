using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public abstract class Bullet : MonoBehaviour
    {
        public Vector3 Advance => transform.position + direction * radius;
        
        [FoldoutGroup("Values"), SerializeField] protected float radius;
        [FoldoutGroup("Values"), SerializeField] protected float speed;
        [FoldoutGroup("Values"), SerializeField] protected LayerMask hitMask;

        protected float actualSpeed;
        protected Vector3 direction = Vector3.forward;

        void Awake() => actualSpeed = speed;
        
        public virtual void Shoot(Aim aim, EventArgs args)
        {
            transform.position = aim.firepoint;
            direction = aim.direction.normalized;
        }

        protected virtual void Update()
        {
            var length = actualSpeed * Time.deltaTime;
            var ray = new Ray(transform.position + direction * radius, direction);

            if (Physics.Raycast(ray, out var hit, length, hitMask)) OnHit(hit);
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