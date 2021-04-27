using UnityEngine;

namespace Chrome.Retro
{
    public abstract class RetGun : ScriptableObject
    {
        public virtual void Begin(IIdentity identity, Collider target, InteractionHub hub) { }
        public abstract bool Use(IIdentity identity, Collider target, InteractionHub hub);
        public virtual void End(IIdentity identity, Collider target, InteractionHub hub) { }
    }
}