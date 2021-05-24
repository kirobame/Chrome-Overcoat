using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TaskedBullet : TaskedProjectile
    {
        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float appearance = 0.15f;
        [FoldoutGroup("Values"), SerializeField] private float damage;
        
        [FoldoutGroup("References"), SerializeField] private PoolableVfx impactVfx;
        [FoldoutGroup("References"), SerializeField] private GameObject graph;
        [FoldoutGroup("References"), SerializeField] private TrailRenderer trail;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void OnShoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            base.OnShoot(source, fireAnchor, direction, packet);
            
            var board = this.packet.Get<IBlackboard>();
            board.Set("dir", direction.normalized);
            board.Set("time", 5.0f);
        }

        protected override ITaskTree BuildTree()
        {
            var board = packet.Get<IBlackboard>();
            board.Set<IEnabler>("trail", new RendererEnabler(trail));
            board.Set("graph", graph);
            
            var trailReference = "trail".Reference<IEnabler>();
            var graphReference = "graph".Reference<GameObject>();
                
            return new ProjectileNode().Append(
                new Enable(true, trailReference).Mask(0b_0001).Append(
                    new Delay(appearance).Append(
                        new SetActive(true, graphReference))),
                new RootNode().Mask(0b_0010).Append(
                    new Timer("time".Reference<float>()).Append(
                        new SetActive(false, identity.Value.Transform.gameObject.Cache()).Mask(0b_0001))),
                new LinearMove(0.015f, "dir".Reference<Vector3>(), speed.Cache(), new PackettedValue<HashSet<Collider>>(), "self".Reference<Transform>()).Mask(0b_0010),
                new Damage(damage.Cache()).Mask(0b_0100).Append(
                    new PlayVfxImpact(impactVfx).Append(
                        new EndProjectile())),
                new SetActive(false, graphReference).Mask(0b_1000).Append(
                    new Delay(0.75f).Append(
                        new Enable(false, trailReference))));
        }
    }
}