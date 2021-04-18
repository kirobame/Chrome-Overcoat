using UnityEngine;

namespace Chrome
{
    public class CameraControl : MonoBehaviour, ILifebound
    {
        private Transform parent;

        void Awake() => parent = transform.parent;

        public void Bootup() => transform.SetParent(parent);
        public void Shutdown() => transform.SetParent(null);
    }
}