using System;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class Shoot : ProxyNode
    {
        private IValue<Vector3> direction;
        private IValue<Transform> fireAnchor;
        
        public Shoot(IValue<Vector3> direction, IValue<Transform> fireAnchor, GenericPoolable bulletPrefab, PoolableVfx muzzleFlashPrefab)
        {
            this.direction = direction;
            this.fireAnchor = fireAnchor;
            
            this.bulletPrefab = bulletPrefab;
            this.muzzleFlashPrefab = muzzleFlashPrefab;
        }
        
        private GenericPoolable bulletPrefab;
        private PoolableVfx muzzleFlashPrefab;
        
        protected override void OnUpdate(Packet packet)
        {
            if (direction.IsValid(packet) && fireAnchor.IsValid(packet))
            {
                SpawnMuzzleFlash(packet, fireAnchor.Value);
                ShootBullet(packet, fireAnchor.Value, direction.Value);
            }

            IsDone = true;
        }

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
            var bulletPool = Repository.Get<GenericPool>(Pool.Bullet);
            var bulletInstance = bulletPool.CastSingle<Bullet>(bulletPrefab);

            var board = packet.Get<Blackboard>();
            if (!board.TryGet<float>("charge", out var force)) force = 1.0f;
            
            Events.ZipCall(PlayerEvent.OnFire, force);
            Events.ZipCall<byte,float>(GaugeEvent.OnGunFired, (byte)(bulletPrefab.name.Contains("Energy") ? 1 : 0), force);
            
            bulletInstance.Shoot(new Aim() { direction = direction, firepoint = fireAnchor.position}, new WrapperArgs<float>(force));
        }
    }
}