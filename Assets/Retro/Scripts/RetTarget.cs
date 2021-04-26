using System.Collections.Generic;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetTarget : MonoBehaviour, ILifebound, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        public InteractionHub InteractionHub => interactionHub;
        public Collider Collider => collider;

        [FoldoutGroup("Dependencies"), SerializeField] private InteractionHub interactionHub;
        [FoldoutGroup("Dependencies"), SerializeField] private new Collider collider;
        
        protected virtual void Awake()
        {
            if (!Repository.Exists(RetReference.Targets)) Repository.Set(RetReference.Targets, new List<RetTarget>());
            Repository.AddTo(RetReference.Targets, this);
        }

        public void Bootup() => Events.ZipCall<RetTarget>(RetEvent.OnTargetSpawn, this);
        public void Shutdown() => Events.ZipCall<RetTarget>(RetEvent.OnTargetDeath, this);
    }
}