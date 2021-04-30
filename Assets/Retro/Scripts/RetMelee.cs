using Flux.Audio;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetMelee", menuName = "Chrome Overcoat/Retro/Guns/Melee")]
    public class RetMelee : RetGun
    {
        [FoldoutGroup("Values"), SerializeField] private float delay;
        [FoldoutGroup("Values"), SerializeField] private float damage;
        [FoldoutGroup("Values"), SerializeField] private byte category;

        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx hitVfxPrefab;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage audioPackage;

        private float timer;
        
        public override void Begin(IIdentity identity, Collider target, InteractionHub hub)
        {
            timer = delay;
            
            var impactPool = Repository.Get<VfxPool>(Pool.Impact);
            var hitPoolable = impactPool.RequestSinglePoolable(hitVfxPrefab);
            
            hitPoolable.transform.position = target.bounds.center;
            hitPoolable.Value.Play();

            var snapshot = identity.Packet.Save();
            identity.Packet.Set(category);
            hub.Relay<IDamageable>(damageable => { damageable.Hit(identity, damage, identity.Packet); });
            identity.Packet.Load(snapshot);
        }

        public override bool Use(IIdentity identity, Collider target, InteractionHub hub)
        {
            timer -= Time.deltaTime;
            return timer <= 0.0f;
        }
    }
}