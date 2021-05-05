using System;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Health : MonoBehaviour, IDamageable, ILifebound, ILink<IIdentity>
    {
        public event Action<float, float> onChange; 
        
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

        void Awake() => Bootup();

        public void Bootup() => health = maxHealth;
        public void Shutdown() { }
        
        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var difference = health - amount;
            var damage = difference < 0 ? amount + difference : amount;
            
            health = Mathf.Clamp(health - damage, 0.0f, maxHealth);
            onChange?.Invoke(health, maxHealth);

            var sourceType = source.Packet.Get<byte>();
            if (identity.Faction == Faction.Player) Events.ZipCall<float,byte>(GaugeEvent.OnDamageReceived, damage, sourceType);
            else if (source.Faction == Faction.Player)
            {
                var type = identity.Packet.Get<byte>();
                
                Events.ZipCall<byte,float,byte>(GaugeEvent.OnDamageInflicted, type, damage, sourceType);
                if (health == 0) Events.ZipCall<byte,byte>(GaugeEvent.OnKill, type, sourceType);
            }
            
            if (health == 0) link.End();
        }

        public void Bootup()
        {
            health = maxHealth;
        }

        public void Shutdown()
        {
        }
    }
}