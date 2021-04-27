using System.Linq;
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
        
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx impactVfx;

        private float timer;
        private Vector3 damping;
        
        private bool hasTarget;
        private Collider target;
        private InteractionHub hub;
        
        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            base.Shoot(source, fireAnchor, direction, packet);

            var board = source.Packet.Get<IBlackboard>();
            var detection = board.Get<RetDetectionControl>(RetPlayerBoard.REF_DETECTION);

            hasTarget = false;
            target = null;
            if (detection.Targets.Any())
            {
                foreach (var possibleTarget in detection.Targets)
                {
                    if (!possibleTarget.TryGetComponent<InteractionHub>(out hub) || hub.Identity.Faction == identity.Faction) continue;

                    hasTarget = true;
                    target = possibleTarget;

                    break;
                }
            }

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
            }
            
            base.Update();
        }

        protected override void OnHit(RaycastHit hit)
        {
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var vfxPoolable = vfxPool.RequestSinglePoolable(impactVfx);
            
            vfxPoolable.transform.localScale = Vector3.one;

            if (hit.collider == target || hit.collider.TryGetComponent<InteractionHub>(out hub) && hub.Identity.Faction != identity.Faction)
            {
                identity.Packet.Set(hit);
                    
                hub.Relay<IDamageable>(damageable =>
                {
                    if (damageable.Identity.Faction == identity.Faction) return;
                    damageable.Hit(identity, damage, identity.Packet);
                });
                
                vfxPoolable.transform.SetParent(hit.transform);
            }

            vfxPoolable.transform.position = hit.point;
            vfxPoolable.transform.rotation = Quaternion.LookRotation(hit.normal);
            vfxPoolable.Value.Play();
            
            transform.root.gameObject.SetActive(false);
        }
    }
}