using System;
using Flux;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Chrome
{
    public class Shield : MonoBehaviour, IDamageable, ILink<IIdentity>
    {
        public IIdentity Identity => identity;
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        public void Hit(IIdentity source, float amount, Packet packet)
        {
            Debug.Log("Shield Hit");
        }
    }
}
