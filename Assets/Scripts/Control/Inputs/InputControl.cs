using System;
using UnityEngine;

namespace Chrome
{
    public abstract class InputControl<T> : MonoBehaviour, IInstaller, ILifebound where T : MonoBehaviour
    {
        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;

        void OnDestroy() => onDestruction?.Invoke(this);
        
        public virtual void Bootup() => enabled = true;
        public virtual void Shutdown() => enabled = false;

        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set<T>(this as T);
    }
}