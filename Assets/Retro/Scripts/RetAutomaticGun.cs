﻿using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetAutomaticGun : RetGun
    {
        [FoldoutGroup("Values"),SerializeField] private float delay;
        [FoldoutGroup("Values"),SerializeField] private GenericPoolable projectilePrefab;
        [FoldoutGroup("Values"),SerializeField] private PoolableVfx muzzleFlashPrefab;
        
        protected float timer;

        public override void Begin(IIdentity identity, Collider target, InteractionHub hub) => timer = delay;

        public override bool Use(IIdentity identity, Collider target, InteractionHub hub)
        {
            timer -= Time.deltaTime;
            return timer <= 0.0f;
        }

        protected void Shoot(IIdentity identity, Vector3 direction, Transform fireAnchor)
        {
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
    }
}