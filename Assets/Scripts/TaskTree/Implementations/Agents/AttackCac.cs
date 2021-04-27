using UnityEngine;

namespace Chrome
{
    public class AttackCac : ProxyNode
    {
        public AttackCac(IIdentity source, IValue<Transform> aimTr, float damages)
        {
            origin = aimTr;
            identity = source;
            this.damages = damages;
        }

        private IValue<Transform> origin;
        private IIdentity identity;
        private float damages;

        protected override void OnUpdate(Packet packet)
        {
            var length = 3f;
            var ray = new Ray(origin.Value.position, origin.Value.forward);

            if (Physics.Raycast(ray, out var hit, length)) OnHit(hit);

            Debug.Log("Attack");
            isDone = true;
        }

        void OnHit(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<InteractionHub>(out var hub))
            {
                identity.Packet.Set(hit);

                hub.Relay<IDamageable>(damageable =>
                {
                    if (damageable.Identity.Faction == identity.Faction) return;
                    damageable.Hit(identity, damages, identity.Packet);
                });
            }
        }
    }
}