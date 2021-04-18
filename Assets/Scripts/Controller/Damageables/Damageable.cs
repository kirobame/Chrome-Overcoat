using Flux;
using UnityEngine;

namespace Chrome
{
    public class Damageable : MonoBehaviour, ILifebound, IDamageable
    {
        public IDamageable Implementation => implementation;
        [SerializeReference] private IDamageable implementation;

        void Awake() => Bootup();

        public void Bootup()
        {
            if (!(implementation is IBootable bootable)) return;
            bootable.Bootup();
        }
        public void Shutdown() { }
        
        public void Hit(RaycastHit hit, float damage) => implementation.Hit(hit, damage);
    }
}