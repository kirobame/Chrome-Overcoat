using System;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Projectile : Bullet
    {
        [FoldoutGroup("Values"), SerializeField] private float damage;
        [FoldoutGroup("Values"), SerializeField] private float deactivationTime;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx impactVfx;
        [FoldoutGroup("Feedbacks"), SerializeField] private GameObject graph;
        [FoldoutGroup("Feedbacks"), SerializeField] private TrailRenderer trail;

        private Coroutine routine;
        
        private bool hasHit;
        private float deactivationTimer;

        public override void Shoot(Aim aim, EventArgs args)
        {
            deactivationTimer = deactivationTime;
            hasHit = false;
            
            base.Shoot(aim, args);

            trail.enabled = true;
            routine = StartCoroutine(Routines.DoAfter(() =>
            {
                graph.SetActive(true);
                routine = null;

            }, 0.15f));
        }

        protected override void Update()
        {
            if (hasHit) return;
            
            deactivationTimer -= Time.deltaTime;
            if (deactivationTimer <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            base.Update();
        }

        protected override void OnHit(RaycastHit hit)
        {
            if (hasHit) return;
            
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfxPoolable = vfxPool.RequestSinglePoolable(impactVfx);
            
            vfxPoolable.transform.localScale = Vector3.one;
            
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                vfxPoolable.transform.SetParent(hit.transform);
                damageable.Hit(hit, damage);
            }
            
            vfxPoolable.transform.position = hit.point;
            vfxPoolable.transform.rotation = Quaternion.LookRotation(hit.normal);
            vfxPoolable.Value.Play();

            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
             
            hasHit = true;
            graph.SetActive(false);
            
            Routines.Start(Routines.DoAfter(() =>
            {
                trail.enabled = false;
                gameObject.SetActive(false);

            }, 0.75f));
        }
    }
}