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
        [SerializeField] private Animator animator;
        [SerializeField] private GenericPoolable bullet;
        [SerializeField] private float force;

        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            animator.SetTrigger("Shoot");
            
            var bulletPool = Repository.Get<GenericPool>(Pool.Bullet);
            var bulletInstance = bulletPool.CastSingle<Bullet>(bullet);
            
            bulletInstance.Shoot(aim, args);

            if (args is IWrapper<float> forceArgs) Events.ZipCall(PlayerEvent.OnFire, Mathf.Lerp(force * 0.25f, force, forceArgs.Value));
            else Events.ZipCall(PlayerEvent.OnFire, force);
            
            return args;
        }
    }
}