using System;
using Flux;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Life : IDamageable, IBootable, IInjectable<Damageable>
    {
        [SerializeField] private Lifetime link;
        [SerializeField] private float maxHealth;

        private Damageable damageable;
        private float health;

        public void Bootup() => health = maxHealth;
        public void Inject(Damageable damageable) => this.damageable = damageable;
        
        public void Hit(RaycastHit hit, float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                if (damageable != null && damageable.Type != 10) Events.ZipCall<byte>(GaugeEvent.OnKill, damageable.Type);
                link.End();
            }
        }
    }
}