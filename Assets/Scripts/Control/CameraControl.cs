using System;
using UnityEngine;

namespace Chrome
{
    public class CameraControl : MonoBehaviour, ILifebound
    {
        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;
        
        private Transform parent;

        void Awake() => parent = transform.parent;
        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup() => transform.SetParent(parent);
        public void Shutdown() => transform.SetParent(null);
    }
}