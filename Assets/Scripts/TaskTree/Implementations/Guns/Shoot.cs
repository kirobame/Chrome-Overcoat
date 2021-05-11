using System;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class Shoot : TaskedNode
    {
        public Shoot(IValue<Vector3> direction, IValue<Transform> fireAnchor, GenericPoolable projectilePrefab, PoolableVfx muzzleFlashPrefab)
        {
            this.direction = direction;
            this.fireAnchor = fireAnchor;
            
            this.projectilePrefab = projectilePrefab;
            this.muzzleFlashPrefab = muzzleFlashPrefab;
        }
        public Shoot(IValue<Vector3> direction, IValue<Transform> fireAnchor, IValue<Collider> collider, GenericPoolable projectilePrefab, PoolableVfx muzzleFlashPrefab)
        {
            this.direction = direction;
            this.fireAnchor = fireAnchor;
            this.collider = collider;
            
            this.projectilePrefab = projectilePrefab;
            this.muzzleFlashPrefab = muzzleFlashPrefab;
        }
        
        private IValue<Collider> collider = new EmptyValue<Collider>();
        private IValue<Vector3> direction;
        private IValue<Transform> fireAnchor;
        
        private GenericPoolable projectilePrefab;
        private PoolableVfx muzzleFlashPrefab;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void OnUse(Packet packet)
        {
            if (direction.IsValid(packet) && fireAnchor.IsValid(packet))
            {
                SpawnMuzzleFlash(packet, fireAnchor.Value);
                ShootBullet(packet, fireAnchor.Value, direction.Value);
            }

            isDone = true;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        private void SpawnMuzzleFlash(Packet packet, Transform fireAnchor)
        {
            var muzzleFlashPool = Repository.Get<VfxPool>(Pool.MuzzleFlash);
            var muzzleFlashInstance = muzzleFlashPool.RequestSinglePoolable(muzzleFlashPrefab);

            muzzleFlashInstance.transform.localScale = Vector3.one;
            muzzleFlashInstance.transform.parent = fireAnchor;
            muzzleFlashInstance.transform.position = fireAnchor.position;
            muzzleFlashInstance.transform.rotation = Quaternion.LookRotation(fireAnchor.forward);
            muzzleFlashInstance.Value.Play();
        }
        
        private void ShootBullet(Packet packet, Transform fireAnchor, Vector3 direction)
        {
            var projectilePool = Repository.Get<GenericPool>(Pool.Projectile);
            var projectileInstance = projectilePool.CastSingle<Projectile>(projectilePrefab);

            var board = packet.Get<IBlackboard>();
            float force;
            
            if (!board.TryGet<bool>("charge.isUsed", out var isUsed) || !isUsed) force = 0.25f;
            else force = board.Get<float>("charge");

            var type = board.Get<byte>("type");
            if (type == 10)
            {
                Events.ZipCall(PlayerEvent.OnFire, force);
                Events.ZipCall<byte,float>(GaugeEvent.OnGunFired, (byte)(projectilePrefab.name.Contains("Energy") ? 0 : 1), force);
            }
            
            packet.Set(force);
            projectileInstance.Shoot(packet.Get<IIdentity>(), fireAnchor.position, direction.normalized, packet);

            if (collider.IsValid(packet)) projectileInstance.Ignore(collider.Value);
        }
    }
}