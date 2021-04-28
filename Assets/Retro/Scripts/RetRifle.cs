using System;
using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetRifle", menuName = "Chrome Overcoat/Retro/Guns/Rifle")]
    public class RetRifle : RetAutomaticGun
    {
        public override void Begin(IIdentity identity, Collider target, InteractionHub hub)
        {
            base.Begin(identity, target, hub);
            
            var direction = identity.Packet.Get<Vector3>();
            var fireAnchor = identity.Packet.Get<Transform>();
            
            Shoot(identity, direction, fireAnchor);
        }
    }
}