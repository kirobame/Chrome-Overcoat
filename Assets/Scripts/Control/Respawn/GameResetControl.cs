using System;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class GameResetControl : MonoBehaviour, ILifebound
    {
        public event Action<ILifebound> onDestruction;

        public bool IsActive => true;
        
        private bool state;
        
        void Awake()
        {
            Events.Subscribe(GlobalEvent.OnStart, OnStart);
            Events.Subscribe(GlobalEvent.OnReset, OnReset);
        }
        void OnDestroy()
        {
            Events.Unsubscribe(GlobalEvent.OnStart, OnStart);
            Events.Unsubscribe(GlobalEvent.OnReset, OnReset);
            
            onDestruction?.Invoke(this);
        }

        public void Bootup(byte code) { }
        public void Shutdown(byte code)
        {
            if (!state) return;
            Events.ZipCall(GlobalEvent.OnReset, "You died");
        }

        //--------------------------------------------------------------------------------------------------------------/

        void OnStart() => state = true;
        void OnReset() => state = false;
    }
}