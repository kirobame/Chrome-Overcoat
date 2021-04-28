using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    [CreateAssetMenu(fileName = "RetShotgun", menuName = "Chrome Overcoat/Retro/Guns/Shotgun")]
    public class RetShotgun : RetAutomaticGun
    {
        [FoldoutGroup("Values"), SerializeField] private int pallet;
        [FoldoutGroup("Values"), SerializeField] private float cone;
        
        public override void Begin(IIdentity identity, Collider target, InteractionHub hub)
        {
            base.Begin(identity, target, hub);
            
            var direction = identity.Packet.Get<Vector3>();
            var fireAnchor = identity.Packet.Get<Transform>();

            var baseDirection = Quaternion.AngleAxis(-cone * 0.5f, Vector3.up) * direction;
            var step = cone / (pallet - 1);

            for (var i = 0; i < pallet; i++)
            {
                direction = Quaternion.AngleAxis(step * i, Vector3.up) * baseDirection;
                Shoot(identity, direction, fireAnchor);
            }
        }
    }
}