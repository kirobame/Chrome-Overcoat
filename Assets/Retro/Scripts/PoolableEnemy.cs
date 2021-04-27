using System;
using Flux.Data;
using UnityEngine;

namespace Chrome.Retro
{
    public class PoolableEnemy : Poolable<Identity>
    {
        public Action<PoolableEnemy> onDeath;

        public override void Reboot() => onDeath?.Invoke(this);
    }
}