using System;
using Flux;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class RefreshToken
    {
        public event Action onActivation;
        
        [SerializeField] private DynamicFlag address;

        public void Bootup()
        {
            address.Bootup();
            Events.Subscribe(address.Value, Activate);
        }
        public void Shutdown() => Events.Unsubscribe(address.Value, Activate);

        private void Activate() => onActivation?.Invoke();
    }
}