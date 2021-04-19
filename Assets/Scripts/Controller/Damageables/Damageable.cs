using Flux;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class Damageable : MonoBehaviour, ILifebound, IDamageable
    {
        public IDamageable Implementation => implementation;
        public byte Type => type;

        [SerializeField] private byte type;
        [SerializeReference] private IDamageable implementation;
        
        void Awake()
        {
            if (implementation is IInjectable<Damageable> injectable) injectable.Inject(this);
            Bootup();
        }

        public void Bootup()
        {
            if (!(implementation is IBootable bootable)) return;
            bootable.Bootup();
        }
        public void Shutdown() { }
        
        public void Hit(RaycastHit hit, float damage)
        {
            if (type == 10) Events.ZipCall<float>(GaugeEvent.OnDamageReceived, damage);
            else Events.ZipCall<byte,float>(GaugeEvent.OnDamageInflicted, type, damage);
            
            implementation.Hit(hit, damage);
        }
    }
}