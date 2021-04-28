using System;
using Flux.Data;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPoolableEnemy : Poolable<Identity>
    {
        public Action<RetPoolableEnemy> onDeath;
            
        public override void Reboot() => onDeath?.Invoke(this);
    }
}