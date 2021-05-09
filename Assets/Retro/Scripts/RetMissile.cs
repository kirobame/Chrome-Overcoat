using System.Linq;
using Flux.Audio;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetMissile : StandardProjectile
    {
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        [FoldoutGroup("Values"), SerializeField] private float damage;
        [FoldoutGroup("Values"), SerializeField] private float duration;
        [FoldoutGroup("Values"), SerializeField] private float refresh;
        [FoldoutGroup("Values"), SerializeField] private float detection;
        [FoldoutGroup("Values"), Range(-1.0f, 1.0f), SerializeField] private float tracking;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx impactVfx;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage sound;
        
        private float timer;
        private Vector3 damping;
        
        private bool hasTarget;
        private Collider target;
        private InteractionHub hub;

        private float check;
        private Collider[] results;

        protected override void Awake()
        {
            base.Awake();
            results = new Collider[3];
        }

        protected override void OnShoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            identity.Value.Copy(source);
            
            base.Shoot(source, fireAnchor, direction, packet);
            
            hasTarget = false;
            target = null;
            
            TryGetTarget();
            timer = duration;
        }

        protected override void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                transform.root.gameObject.SetActive(false);
                return;
            }
            
            if (hasTarget)
            {
                var direction = Vector3.Normalize(target.transform.position.Flatten() - transform.position.Flatten());
                this.direction = Vector3.SmoothDamp(this.direction, direction, ref damping, smoothing);

                if (Vector3.Dot(transform.forward, direction) <= tracking)
                {
                    hasTarget = false;
                    target = null;

                    check = refresh;
                }
            }
            
            base.Update();

            if (!hasTarget)
            {
                check -= Time.deltaTime;
                if (check <= 0.0f) TryGetTarget();
            }
        }

        private void TryGetTarget()
        {
            var count = Physics.OverlapSphereNonAlloc(transform.position, detection, results, LayerMask.GetMask("Entity"));
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    if (!results[i].TryGetComponent<InteractionHub>(out hub) || hub.Identity.Faction == identity.Value.Faction) continue;
                
                    hasTarget = true;
                    target = results[i];
                
                    break;
                }
            }

            check = refresh;
        }

        protected override void OnHit(RaycastHit hit)
        {
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfxPoolable = vfxPool.RequestSinglePoolable(impactVfx);
            
            vfxPoolable.transform.localScale = Vector3.one;

            sound.Play();
            if (hit.collider == target || hit.collider.TryGetComponent<InteractionHub>(out hub) && hub.Identity.Faction != identity.Value.Faction)
            {
                packet.Set(hit);
                hub.RelayDamage(identity.Value, damage);
                
                vfxPoolable.transform.SetParent(hit.transform);
            }

            vfxPoolable.transform.position = hit.point;
            vfxPoolable.transform.rotation = Quaternion.LookRotation(hit.normal);
            vfxPoolable.Value.Play();
            
            transform.root.gameObject.SetActive(false);
        }
    }
}