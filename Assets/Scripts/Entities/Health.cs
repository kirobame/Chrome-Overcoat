using System;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Health : MonoBehaviour, IDamageable, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        [BoxGroup("Dependencies"), SerializeField] private Lifetime link;
        
        [FoldoutGroup("Values"), SerializeField] private float maxHealth;
        
        private float health;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake() => health = maxHealth;
        
        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var difference = health - amount;
            var damage = difference < 0 ? amount + difference : amount;
            health -= damage;
            
            if (identity.Faction == Faction.Player) Events.ZipCall<float>(GaugeEvent.OnDamageReceived, damage);
            else if (source.Faction == Faction.Player)
            {
                var type = identity.Packet.Get<byte>();
                
                Events.ZipCall<byte, float>(GaugeEvent.OnDamageInflicted, type, damage);
                if (health <= 0) Events.ZipCall<byte>(GaugeEvent.OnKill, type);
            }
            
            if (health <= 0) link.End();
        }
    }
}