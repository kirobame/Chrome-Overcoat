using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public abstract class StandardProjectile : Projectile
    {
        public float Speed { get; protected set; }
        public Vector3 Advance => transform.position + direction * radius;
        
        [FoldoutGroup("Values"), SerializeField] protected float radius;
        [FoldoutGroup("Values"), SerializeField] protected float speed;
        [FoldoutGroup("Values"), SerializeField] protected LayerMask hitMask;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake() => Speed = speed;

        protected virtual void Update()
        {
            var length = Speed * Time.deltaTime;
            var ray = new Ray(Advance, direction);

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