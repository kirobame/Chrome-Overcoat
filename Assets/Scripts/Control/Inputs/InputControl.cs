using System;
using UnityEngine;

namespace Chrome
{
    public class InputControl : MonoBehaviour, ILifebound
    {
        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;

        void OnDestroy() => onDestruction?.Invoke(this);
        
        public virtual void Bootup()
        {
            enabled = true;
        }
        public virtual void Shutdown() => enabled = false;
    }
}