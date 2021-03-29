using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Shooter : GunPart
    {
        [SerializeField] private GenericPoolable bullet;

        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            var bulletPool = Repository.Get<GenericPool>(Pool.Bullet);
            var bulletInstance = bulletPool.CastSingle<Bullet>(bullet);
            
            bulletInstance.Shoot(aim, args);
            return args;
        }
    }
}