using UnityEngine;

namespace Chrome
{
    public class AttackCac : TaskedNode
    {
        public AttackCac(IIdentity source, IValue<Transform> aimTr, float damage)
        {
            origin = aimTr;
            identity = source;
            this.damage = damage;
        }

        private IValue<Transform> origin;
        private IIdentity identity;
        private float damage;

        protected override void OnUse(Packet packet)
        {
            var length = 3f;
            var ray = new Ray(origin.Value.position, origin.Value.forward);

            if (Physics.Raycast(ray, out var hit, length)) OnHit(hit);
            
            isDone = true;
        }

        void OnHit(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<InteractionHub>(out var hub))
            {
                identity.Packet.Set(hit);
                hub.RelayDamage(identity, damage);
            }
        }
    }
}