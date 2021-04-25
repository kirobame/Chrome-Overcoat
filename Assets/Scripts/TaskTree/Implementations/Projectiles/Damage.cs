using UnityEngine;

namespace Chrome
{
    public class Damage : ProxyNode
    {
        public Damage(IValue<float> amount) => this.amount = amount;

        private IValue<float> amount;
  
        protected override void OnUpdate(Packet packet)
        {
            if (amount.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();
                var identity = packet.Get<IIdentity>();

                var hit = board.Get<CollisionHit<Transform>>("hit");
                if (hit.Collider.TryGetComponent<InteractionHub>(out var hub))
                {
                    packet.Set(hit);
                    hub.Relay<IDamageable>(damageable =>
                    {
                        if (damageable.Identity.Faction == identity.Faction) return;
                        damageable.Hit(identity, amount.Value, packet);
                    });
                }
            }

            isDone = true;
        }
    }
}