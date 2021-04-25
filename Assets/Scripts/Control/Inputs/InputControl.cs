using UnityEngine;

namespace Chrome
{
    public class InputControl : MonoBehaviour, ILifebound
    {
        public virtual void Bootup() => enabled = true;
        public virtual void Shutdown() => enabled = false;
    }
}