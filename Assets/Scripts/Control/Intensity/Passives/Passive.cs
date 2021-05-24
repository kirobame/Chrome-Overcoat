using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class Passive : IIntensityPassive
    {
        public Vector2 Range => range;
        
        [SerializeField] private Vector2 range;

        public virtual void Bootup(IIdentity identity) { }

        public virtual void Start(float value, float ratio, IIdentity identity) { }
        public virtual void Update(float value, float ratio, IIdentity identity) { }
        public virtual void End(float value, float ratio, IIdentity identity) { }
        
        public virtual void Shutdown(IIdentity identity) { }
    }
}