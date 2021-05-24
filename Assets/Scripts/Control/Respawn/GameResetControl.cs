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

        void OnDestroy() => onDestruction?.Invoke(this);
 
        public void Bootup(byte code) { }
        public void Shutdown(byte code) => Events.ZipCall(GlobalEvent.OnReset, "You died");
    }
}