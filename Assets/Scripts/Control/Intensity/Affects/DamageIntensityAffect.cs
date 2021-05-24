using System;
using UnityEngine;

namespace Chrome
{
    public class DamageIntensityAffect : IntensityAffect, IDamageable
    {
        protected override void PrepareInjection() => identity = injections.Register(new AnyValue<IIdentity>());

        //--------------------------------------------------------------------------------------------------------------/

        private event Action<IInteraction> onInteractionDestruction;
        event Action<IInteraction> IActive<IInteraction>.onDestruction
        {
            add => onInteractionDestruction += value;
            remove => onInteractionDestruction -= value;
        }

        public IIdentity Identity => identity.Value;
        public bool IsActive => enabled;

        [SerializeField] private float affect;
        [SerializeField] private float separation;
        
        private IValue<IIdentity> identity;

        private float lastHit;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake() => lastHit = -separation * 2.0f;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            onInteractionDestruction?.Invoke(this);
        }

        public void Hit(IIdentity source, float amount, Packet packet)
        {
            var hit = Time.time;
            if (hit - lastHit <= separation) Intensity.affect += affect;

            lastHit = hit;
        }
    }
}