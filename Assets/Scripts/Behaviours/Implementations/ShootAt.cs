using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class ShootAt : ProxyNode
    {
        private IValue<PhysicBody> target;
        private IValue<Transform> fireAnchor;
        
        public ShootAt(IValue<PhysicBody> target, IValue<Transform> fireAnchor, GenericPoolable bulletPrefab, PoolableVfx muzzleFlashPrefab)
        {
            this.target = target;
            this.fireAnchor = fireAnchor;
            
            this.bulletPrefab = bulletPrefab;
            this.muzzleFlashPrefab = muzzleFlashPrefab;
        }
        
        private GenericPoolable bulletPrefab;
        private PoolableVfx muzzleFlashPrefab;
        
        protected override void OnUpdate(Packet packet)
        {
            if (target.IsValid(packet) && fireAnchor.IsValid(packet))
            {
                SpawnMuzzleFlash(fireAnchor.Value);

                var point = target.Value.transform.position + target.Value.Controller.center;
                var direction = Vector3.Normalize(point - fireAnchor.Value.position);
                ShootBullet(fireAnchor.Value, direction);
            }

            IsDone = true;
        }

        private void SpawnMuzzleFlash(Transform fireAnchor)
        {
            var muzzleFlashPool = Repository.Get<VfxPool>(Pool.MuzzleFlash);
            var muzzleFlashInstance = muzzleFlashPool.RequestSinglePoolable(muzzleFlashPrefab);

            muzzleFlashInstance.transform.localScale = Vector3.one;
            muzzleFlashInstance.transform.parent = fireAnchor;
            muzzleFlashInstance.transform.position = fireAnchor.position;
            muzzleFlashInstance.transform.rotation = Quaternion.LookRotation(fireAnchor.forward);
            muzzleFlashInstance.Value.Play();
        }
        
        private void ShootBullet(Transform fireAnchor, Vector3 direction)
        {
            var bulletPool = Repository.Get<GenericPool>(Pool.Bullet);
            var bulletInstance = bulletPool.CastSingle<Bullet>(bulletPrefab);
            
            bulletInstance.Shoot(new Aim() { direction = direction, firepoint = fireAnchor.position}, EventArgs.Empty);
        }
    }
}