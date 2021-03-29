using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Shooter : GunPart
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GenericPoolable bullet;

        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            animator.SetTrigger("Shoot");
            
            var bulletPool = Repository.Get<GenericPool>(Pool.Bullet);
            var bulletInstance = bulletPool.CastSingle<Bullet>(bullet);
            
            bulletInstance.Shoot(aim, args);
            return args;
        }
    }
}