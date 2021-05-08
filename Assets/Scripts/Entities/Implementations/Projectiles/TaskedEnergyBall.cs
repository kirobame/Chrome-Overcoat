using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TaskedEnergyBall : TaskedProjectile
    {
        [FoldoutGroup("Values"), SerializeField] private Vector2 damage;
        [FoldoutGroup("Values"), SerializeField] private Vector2 otherSpeed;
        [FoldoutGroup("Values"), SerializeField] private Vector2 size;
        
        [FoldoutGroup("References"), SerializeField] private PoolableVfx impactVfx;
        [FoldoutGroup("References"), SerializeField] private PoolableVfx bounceVfx;
        [FoldoutGroup("References"), SerializeField] private Transform graph;

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("dir", direction.normalized);
            
            if (packet.TryGet<float>(out var charge))
            {
                board.Set("damage", Mathf.Lerp(damage.x, damage.y, charge));
                board.Set("speed", Mathf.Lerp(otherSpeed.x, otherSpeed.y, charge));
                board.Set("size", new Vector2(size.x, Mathf.Lerp(size.x, size.y, charge)));
            }
            else
            {
                board.Set("damage", damage.x);
                board.Set("speed", otherSpeed.x);
                board.Set("size", new Vector2(size.x, size.x));
            }
            
            base.Shoot(source, fireAnchor, direction, packet);
        }

        protected override ITaskTree BuildTree()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("time", 5.0f);

            return new ProjectileNode().Append(
                new ScaleBetween(0.3f, "size".Reference<Vector2>(), graph.Cache()).Mask(0b_0001),
                new RootNode().Mask(0b_0010).Append(
                    new Timer("time".Reference<float>()).Append(
                        new SetActive(false, identity.Transform.gameObject.Cache()).Mask(0b_0001))),
                new LinearMove(0.015f, "dir".Reference<Vector3>(), "speed".Reference<float>(), new PackettedValue<HashSet<Collider>>(), "self".Reference<Transform>()).Mask(0b_0010),
                new Damage("damage".Reference<float>()).Mask(0b_0100).Append(
                    new Counter(3).Append(
                        new Reflect("dir".Reference<Vector3>()).Mask(0b_0010).Append(
                            new PlayVfxImpact(bounceVfx).Append(
                                new SetLocalReference<float>("time", 5.0f))),
                        new PlayVfxImpact(impactVfx).Mask(0b_0001).Append(
                            new EndProjectile()))));
        }
    }
}