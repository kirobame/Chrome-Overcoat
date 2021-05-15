using UnityEngine;

namespace Chrome
{
    public class Damage : TaskNode
    {
        public Damage(IValue<float> amount) => this.amount = amount;

        private IValue<float> amount;
  
        protected override void OnUse(Packet packet)
        {
            if (amount.IsValid(packet))
            {
                var board = packet.Get<IBlackboard>();
                var identity = packet.Get<IIdentity>();

                var hit = board.Get<CollisionHit<Transform>>("hit");
                if (hit.Collider.TryGetComponent<InteractionHub>(out var hub))
                {
                    packet.Set(hit);
                    hub.Relay<IDamageable>((damageable, depth) =>
                    {
                        if (damageable.Identity.Faction == identity.Faction) return false;
                        
                        damageable.Hit(identity, amount.Value, packet);
                        return true;
                    });
                }
            }

            isDone = true;
        }
    }
}