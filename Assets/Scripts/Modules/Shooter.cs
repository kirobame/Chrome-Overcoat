using System;
using Flux;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Shooter : GunPart
    {
        [SerializeField] private byte type;
        [SerializeField] private Animator animator;
        [SerializeField] private GenericPoolable bullet;
        [SerializeField] private float force;

        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            animator.SetTrigger("Shoot");
            
            var bulletPool = Repository.Get<GenericPool>(Pool.Bullet);
            var bulletInstance = bulletPool.CastSingle<Bullet>(bullet);
            
            bulletInstance.Shoot(aim, args);

            float force;
            if (args is IWrapper<float> forceArgs) force = Mathf.Lerp(this.force * 0.25f, this.force, forceArgs.Value);
            else force = this.force;
            
            Events.ZipCall(PlayerEvent.OnFire, force);
            Events.ZipCall<byte,float>(GaugeEvent.OnGunFired, type, force);
            
            return args;
        }
    }
}