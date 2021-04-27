using System;
using Flux.Data;
using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetRifle", menuName = "Chrome Overcoat/Retro/Guns/Rifle")]
    public class RetRifle : RetGun
    {
        [SerializeField] private float delay;
        [SerializeField] private GenericPoolable projectilePrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        
        private float timer;

        public override void Begin(IIdentity identity, Collider target, InteractionHub hub)
        {
            timer = delay;
            
            var board = identity.Packet.Get<IBlackboard>();
            var fireAnchor = board.Get<Transform>("aim.fireAnchor");
            var direction = identity.Packet.Get<Vector3>();
            
            var muzzleFlashPool = Repository.Get<VfxPool>(Pool.MuzzleFlash);
            var muzzleFlashInstance = muzzleFlashPool.RequestSinglePoolable(muzzleFlashPrefab);

            muzzleFlashInstance.transform.localScale = Vector3.one;
            muzzleFlashInstance.transform.parent = fireAnchor;
            muzzleFlashInstance.transform.position = fireAnchor.position;
            muzzleFlashInstance.transform.rotation = Quaternion.LookRotation(fireAnchor.forward);
            muzzleFlashInstance.Value.Play();
            
            var projectilePool = Repository.Get<GenericPool>(Pool.Projectile);
            var projectileInstance = projectilePool.CastSingle<Projectile>(projectilePrefab);
            
            projectileInstance.Shoot(identity, fireAnchor.position, direction, identity.Packet);
        }

        public override bool Use(IIdentity identity, Collider target, InteractionHub hub)
        {
            timer -= Time.deltaTime;
            return timer <= 0.0f;
        }
    }
}