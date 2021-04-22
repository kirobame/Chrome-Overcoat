using System;
using Flux;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Life : IHittable, IBootable
    {
        public IExtendedIdentity Identity => owner;
        
        [SerializeField] private Entity owner;
        [SerializeField] private Lifetime link;
        [SerializeField] private float maxHealth;
        
        private float health;

        public void Bootup() => health = maxHealth;
        
        public void Hit(IIdentity identity, HitMotive motive, EventArgs args)
        {
            if (motive != HitMotive.Damage || !(args is IWrapper<float> damageArgs)) return;

            var difference = health - damageArgs.Value;
            var damage = difference < 0 ? damageArgs.Value + difference : damageArgs.Value;
            health -= damage;
            
            if (owner.Faction == Faction.Player) Events.ZipCall<float>(GaugeEvent.OnDamageReceived, damage);
            else if (identity.Faction == Faction.Player)
            {
                var type = owner.Board.Get<byte>("type");
                
                Events.ZipCall<byte, float>(GaugeEvent.OnDamageInflicted, type, damage);
                if (health <= 0) Events.ZipCall<byte>(GaugeEvent.OnKill, type);
            }
            
            if (health <= 0) link.End();
        }
    }
}