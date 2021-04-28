using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public abstract class RetGun : ScriptableObject
    {
        public RetGunModel Model => model;
        public float Spread => spread;
        public float Radius => radius;

        [FoldoutGroup("Embedded"), SerializeField] private RetGunModel model;
        [FoldoutGroup("Embedded"), SerializeField] private float spread;
        [FoldoutGroup("Embedded"), SerializeField] private float radius;
        
        public virtual void Begin(IIdentity identity, Collider target, InteractionHub hub) { }
        public abstract bool Use(IIdentity identity, Collider target, InteractionHub hub);
        public virtual void End(IIdentity identity, Collider target, InteractionHub hub) { }

        public virtual void Interrupt() { }
    }
}