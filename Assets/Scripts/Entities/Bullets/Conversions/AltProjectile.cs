using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class AltProjectile : AltBullet
    {
        [SerializeField] private PoolableVfx impactVfx;
        [SerializeField] private GameObject graph;
        [SerializeField] private TrailRenderer trail;

        public override void Shoot(IIdentity source, Vector3 fireAnchor, Vector3 direction, Packet packet)
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("dir", direction.normalized);
            
            base.Shoot(source, fireAnchor, direction, packet);
        }

        protected override ITaskTree BuildTree()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set<IEnable>("trail", new RendererEnable(trail));
            board.Set("graph", graph);
            
            var trailReference = "trail".Reference<IEnable>();
            var graphReference = "graph".Reference<GameObject>();
                
            return new PewPew().Append(
                new Enable(true, trailReference).Mask(0b_0001).Append(
                    new Delay(0.15f).Append(
                        new SetActive(true, graphReference))),
                new RootNode().Mask(0b_0010).Append(
                    new Timer(5.0f.Cache()).Append(
                        new SetActive(false, identity.Root.gameObject.Cache()).Mask(0b_0001))),
                new LinearMove(radius, "dir".Reference<Vector3>(), speed.Cache(), new PackettedValue<HashSet<Collider>>(), "self".Reference<Transform>()).Mask(0b_0010),
                new Damage(1.0f.Cache()).Mask(0b_0100).Append(
                    new Impact(impactVfx).Append(
                        new End())),
                new SetActive(false, graphReference).Mask(0b_1000).Append(
                    new Delay(0.75f).Append(
                        new Enable(false, trailReference))));
        }
    }
}