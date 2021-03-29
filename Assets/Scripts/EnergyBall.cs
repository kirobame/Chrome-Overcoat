using System;
using System.Collections;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class EnergyBall : Bullet
    {
        [FoldoutGroup("Values"), SerializeField] private float maxSpeed;
        
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

        public override void Shoot(Aim aim, EventArgs args)
        {
            var ratio = ((IWrapper<float>) args).Value;
            
            actualSpeed = Mathf.Lerp(speed, maxSpeed, ratio);
            var size = Mathf.Lerp(range.x, range.y, ratio);
            
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
            
            base.Shoot(aim, args);
        }

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
                PlayImpact(vfxPool.RequestSingle(impactVfx), hit);
                Explode();
               
                return;
            }

            PlayImpact(vfxPool.RequestSingle(bounceVfx), hit);

            bounceCounter--;
            transform.position = hit.point - direction * radius;
            
            direction = Vector3.Reflect(direction, hit.normal);
            direction.Normalize();
            
            deactivationTimer = deactivationTime;
        }

        private void PlayImpact(ParticleSystem vfx, RaycastHit hit)
        {
            vfx.transform.localScale = Vector3.one * (transform.localScale.x * impactSize);
            vfx.transform.position = hit.point;
            vfx.transform.rotation = Quaternion.LookRotation(hit.normal);
            
            vfx.Play();
        }
        
        private void Explode()
        {
            gameObject.SetActive(false);
        }
    }
}