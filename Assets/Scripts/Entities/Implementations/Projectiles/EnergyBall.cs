using System;
using Flux;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class EnergyBall : StandardProjectile
    {
        [FoldoutGroup("Values"), SerializeField] private float maxSpeed;
        [FoldoutGroup("Values"), SerializeField] private Vector2 damages;
        
        [FoldoutGroup("Bounce"), SerializeField] private int bounceCount;
        [FoldoutGroup("Bounce"), SerializeField] private float deactivationTime;
        [FoldoutGroup("Bounce"), SerializeField, Range(0.01f, 3.0f)] private float impactSize;
        [FoldoutGroup("Bounce"), SerializeField] private PoolableVfx bounceVfx;
        [FoldoutGroup("Bounce"), SerializeField] private PoolableVfx impactVfx;
        
        [FoldoutGroup("Size"), SerializeField] private AnimationCurve map;
        [FoldoutGroup("Size"), SerializeField] private Transform sizeTarget;
        [FoldoutGroup("Size"), SerializeField] private float time;
        [FoldoutGroup("Size"), SerializeField] private Vector2 range;

        private Coroutine routine;
        
        private int bounceCounter;
        private float deactivationTimer;

        private float damage;
        
        //--------------------------------------------------------------------------------------------------------------/

        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            identity.Copy(source);

            float size;
            if (packet.TryGet<float>(out var charge))
            {
                damage = Mathf.Lerp(damages.x, damages.y, charge);
                Speed = Mathf.Lerp(speed, maxSpeed, charge);
                size = Mathf.Lerp(range.x, range.y, charge);
            }
            else
            {
                damage = damages.x;
                Speed = speed;
                size = range.x;
            }
            
            sizeTarget.localScale = Vector3.one * range.x;

            if (routine != null) StopCoroutine(routine);
            var margin = new Vector2(range.x, size);
            
            routine = StartCoroutine(Routines.RepeatFor(time, ratio =>
            {
                var scale = Mathf.Lerp(margin.x, margin.y, map.Evaluate(ratio));
                sizeTarget.localScale = Vector3.one * scale;

            }, new YieldFrame()).Chain(Routines.Do(() => routine = null)));
            
            bounceCounter = bounceCount;
            deactivationTimer = deactivationTime;
            
            base.Shoot(source, fireAnchor, direction, packet);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void Update()
        {
            deactivationTimer -= Time.deltaTime;
            if (deactivationTimer <= 0.0f)
            {
                Explode();
                return;
            }
            
            base.Update();
        }

        protected override void OnHit(RaycastHit hit)
        {
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            
            if (bounceCounter <= 0)
            {
                var vfx = PlayImpact(vfxPool.RequestSinglePoolable(impactVfx), hit);
                TryHit(vfx, hit);

                Explode();
                return;
            }

            var hitVfx = PlayImpact(vfxPool.RequestSinglePoolable(bounceVfx), hit);
            TryHit(hitVfx, hit);

            bounceCounter--;
            transform.position = hit.point - direction * radius;
            
            direction = Vector3.Reflect(direction, hit.normal);
            direction.Normalize();
            
            deactivationTimer = deactivationTime;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private PoolableVfx PlayImpact(PoolableVfx vfx, RaycastHit hit)
        {
            vfx.transform.localScale = Vector3.one * (transform.localScale.x * impactSize);
            vfx.transform.position = hit.point;
            vfx.transform.rotation = Quaternion.LookRotation(hit.normal);
            
            vfx.Value.Play();
            return vfx;
        }

        private void TryHit(PoolableVfx vfx, RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<InteractionHub>(out var hub))
            {
                identity.Packet.Set(hit);
                    
                hub.Relay<IDamageable>(damageable =>
                {
                    if (damageable.Identity.Faction == identity.Faction) return;
                    damageable.Hit(identity, damage, identity.Packet);
                });
                    
                vfx.transform.SetParent(hit.transform);
            }
        }
        private void Explode() => gameObject.SetActive(false);
    }
}