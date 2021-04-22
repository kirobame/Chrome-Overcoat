using System;
using Flux;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class Hittable : MonoBehaviour, ILifebound, IHittable
    {
        public IExtendedIdentity Identity => owner;
        public IHittable Implementation => implementation;

        [SerializeField] private Entity owner;
        [SerializeReference] private IHittable implementation;
        
        void Awake() => Bootup();

        public void Bootup()
        {
            if (!(implementation is IBootable bootable)) return;
            bootable.Bootup();
        }
        public void Shutdown() { }
        
        public void Hit(IIdentity identity, HitMotive motive, EventArgs args) =>  implementation.Hit(identity, motive, args);
    }
}