using Flux;
using Flux.Audio;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGrenade : StandardProjectile
    {
        [FoldoutGroup("Values"), SerializeField] private float bonusSpeed;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve gain;
        [FoldoutGroup("Values"), SerializeField] private float duration;
        [FoldoutGroup("Values"), SerializeField] private float splash;
        [FoldoutGroup("Values"), SerializeField] private float splashSize;
        [FoldoutGroup("Values"), SerializeField] private float damage;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve falloff;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx impactVfx;
        [FoldoutGroup("Feedbacks"), SerializeField] private GameObject graph;
        [FoldoutGroup("Feedbacks"), SerializeField] private TrailRenderer trail;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage sound;

        private float timer;
        
        private bool hasHit;
        private Coroutine routine;

        private float cachedSpeed;

        protected override void Awake()
        {
            base.Awake();
            cachedSpeed = speed;
        }
        
        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            speed = cachedSpeed;
            
            identity.Copy(source);
            timer = duration;
            hasHit = false;
            
            graph.transform.localScale = Vector3.zero;
            graph.SetActive(true);
            trail.enabled = true;
            
            routine = StartCoroutine(Routines.RepeatFor(0.6f, ratio =>
            {
                var scale = Mathf.Lerp(0.0f, 0.5f, ratio);
                graph.transform.localScale = Vector3.one * scale;

            }, new YieldFrame()).Chain(Routines.Do(() => routine = null)));
            
            base.Shoot(source, fireAnchor, direction, packet);
        }

        protected override void Update()
        {
            if (hasHit) return;
            
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                transform.root.gameObject.SetActive(false);
                return;
            }

            var success = false;
            var results = Physics.OverlapSphere(transform.position, 0.25f, LayerMask.GetMask("Entity"));
            foreach (var result in results)
            {
                if (!result.TryGetComponent<InteractionHub>(out var hub) || hub.Identity.Faction == identity.Faction) continue;
                
                var distance = Vector3.Distance(transform.position, result.transform.position);
                var ratio = Mathf.Clamp01(distance / splash);
                
                hub.RelayDamage(identity, damage);
                success = true;
            }

            if (success)
            {
                Hit(transform.position);
                return;
            }

            Speed = cachedSpeed + gain.Evaluate(1.0f - timer / duration) * bonusSpeed;
            base.Update();
        }

        protected override void OnHit(RaycastHit hit)
        {
            var results = Physics.OverlapSphere(transform.position, splash, LayerMask.GetMask("Entity"));
            foreach (var result in results)
            {
                if (!result.TryGetComponent<InteractionHub>(out var hub) || hub.Identity.Faction == identity.Faction) continue;
                
                var distance = Vector3.Distance(transform.position, result.transform.position);
                var ratio = Mathf.Clamp01(distance / splash);
                
                hub.RelayDamage(identity, damage);
            }

            Hit(hit.point);
        }

        private void Hit(Vector3 point)
        {
            sound.Play();
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfxPoolable = vfxPool.RequestSinglePoolable(impactVfx);

            vfxPoolable.transform.localScale = Vector3.one * (splash * splashSize);
            vfxPoolable.transform.position = point;
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
                transform.root.gameObject.SetActive(false);

            }, 2.25f));
        }
    }
}