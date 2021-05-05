using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public abstract class RetGun : ScriptableObject
    {
        public string Title => title;
        public Sprite CutIcon => cutIcon;
        public Sprite Icon => icon;   
        
        public RetGunModel Model => model;
        public GenericPoolable Pickup => pickup;
        
        public float Spread => spread;
        public float Radius => radius;

        public int MaxAmmo => maxAmmo;

        [FoldoutGroup("Info"), SerializeField] private string title;
        [FoldoutGroup("Info"), SerializeField] private Sprite cutIcon;
        [FoldoutGroup("Info"), SerializeField] private Sprite icon;
        
        [FoldoutGroup("Embedded"), SerializeField] private RetGunModel model;
        [FoldoutGroup("Embedded"), SerializeField] private GenericPoolable pickup;
        [FoldoutGroup("Embedded"), SerializeField] private float spread;
        [FoldoutGroup("Embedded"), SerializeField] private float radius;

        [FoldoutGroup("Values"), SerializeField] private int maxAmmo;
        
        public virtual void Begin(IIdentity identity, Collider target, InteractionHub hub) { }
        public abstract bool Use(IIdentity identity, Collider target, InteractionHub hub);
        public virtual void End(IIdentity identity, Collider target, InteractionHub hub) { }

        public virtual void Interrupt() { }
    }
}