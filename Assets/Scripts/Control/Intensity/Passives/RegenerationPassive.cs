using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class RegenerationPassive : Passive
    {
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float factor;
        
        public override void Update(float value, float ratio, IIdentity identity)
        {
            var hub = identity.Packet.Get<InteractionHub>();
            hub.Relay<IHealable>((healable, depth) =>
            {
                healable.Heal(identity, map.Evaluate(ratio) * factor * Time.deltaTime, identity.Packet);
                return true;
            });
        }
    }
}