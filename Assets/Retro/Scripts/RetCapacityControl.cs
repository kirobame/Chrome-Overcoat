using System;
using Flux;
using Flux.Audio;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetCapacityControl : InputControl, ILink<IIdentity>
    { 
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        [FoldoutGroup("Dependencies"), SerializeField] private RetMoveControl move;
        
        [FoldoutGroup("Values"), SerializeField] private float maxTime;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve damageMap;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve falloff;
        [FoldoutGroup("Values"), SerializeField] private float damage;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve splashMap;
        [FoldoutGroup("Values"), SerializeField] private float splash;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve sizeMap;
        [FoldoutGroup("Values"), SerializeField] private float size;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve costMap;
        [FoldoutGroup("Values"), SerializeField] private float cost;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve speedMap;
        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        [FoldoutGroup("Feedbacks"), SerializeField] private float trailSmoothing;
        [FoldoutGroup("Feedbacks"), SerializeField] private float targetWidth;
        [FoldoutGroup("Feedbacks"), SerializeField] private TrailRenderer trail;
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx explosionPrefab;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage activation;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage explosion;

        private bool isActive;
        private float timer;
        
        private float speedBoost;
        private float damping;

        private float trailDamping;
        private float trailWidth;

        public override void Shutdown()
        {
            base.Shutdown();
            
            if (isActive)
            {
                timer = 0.0f;
                isActive = false;
            }
        }

        void Update()
        {
            if (!isActive)
            {
                speedBoost = Mathf.SmoothDamp(0.0f, speedBoost, ref damping, smoothing);
                trailWidth = Mathf.SmoothDamp(0.0f, trailWidth, ref trailDamping, trailSmoothing);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                activation.Play();
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                timer = Mathf.Clamp(timer + Time.deltaTime, 0.0f, maxTime);
                var ratio = timer / maxTime;
                
                var speedGoal = speedMap.Evaluate(ratio) * speed;
                
                speedBoost = Mathf.SmoothDamp(speedGoal, speedBoost, ref damping, smoothing);
                trailWidth = Mathf.SmoothDamp(targetWidth, trailWidth, ref trailDamping, trailSmoothing);

                var gauge = Repository.Get<RetGauge>(RetReference.Gauge);
                gauge.Modify(costMap.Evaluate(ratio) * cost);
                
                isActive = true;
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                explosion.Play();
                
                var ratio = timer / maxTime;
                var splash = splashMap.Evaluate(ratio) * this.splash;
                var damage = damageMap.Evaluate(ratio) * this.damage;
                
                var results = Physics.OverlapSphere(transform.position, splash, LayerMask.GetMask("Entity"));
                foreach (var result in results)
                {
                    if (!result.TryGetComponent<InteractionHub>(out var hub) || hub.Identity.Faction == identity.Faction) continue;
                
                    var distance = Vector3.Distance(transform.position, result.transform.position);
                    var splashRatio = Mathf.Clamp01(distance / splash);
                
                    hub.Relay<IDamageable>(damageable =>
                    {
                        if (damageable.Identity.Faction == identity.Faction) return;
                        damageable.Hit(identity, falloff.Evaluate(splashRatio) * damage, identity.Packet);
                    });
                }
            
                var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
                var vfxPoolable = vfxPool.RequestSinglePoolable(explosionPrefab);

                vfxPoolable.transform.localScale = Vector3.one * (sizeMap.Evaluate(ratio) * size);
                vfxPoolable.transform.position = transform.position + Vector3.up * 0.2f;
                vfxPoolable.Value.Play();

                timer = 0.0f;
                isActive = false;
            }
            
            move.speedBoost = speedBoost;

            if (trailWidth <= 0.001f) trail.enabled = false;
            else
            {
                trail.enabled = true;
                trail.widthMultiplier = trailWidth;
            }
        }
    }
}