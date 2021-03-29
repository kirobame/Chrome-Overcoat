using System;
using UnityEngine;

namespace Chrome
{
    public abstract class Bullet : MonoBehaviour
    {
        public Vector3 Advance => transform.position + direction * radius;
        
        [SerializeField] protected float radius;
        [SerializeField] protected float speed;
        [SerializeField] protected LayerMask hitMask;

        protected Vector3 direction = Vector3.forward;

        public void Shoot(Aim aim, EventArgs args)
        {
            transform.position = aim.firepoint;
            direction = aim.direction.normalized;
        }

        void Update()
        {
            var length = speed * Time.deltaTime;
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