using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunNode : ProxyNode
    {
        public RetGunNode(IValue<Vector3> shootDirection, IValue<Transform> fireAnchor, IValue<Collider> target, RetGun gun)
        {
            this.shootDirection = shootDirection;
            this.fireAnchor = fireAnchor;
            this.target = target;

            this.gun = gun;
        }
        
        private IValue<Vector3> shootDirection;
        private IValue<Transform> fireAnchor;
        private IValue<Collider> target;

        private RetGun gun;
        private InteractionHub hub;

        protected override void OnStart(Packet packet)
        {
            if (!shootDirection.IsValid(packet) || !fireAnchor.IsValid(packet) || !target.IsValid(packet) || !target.Value.TryGetComponent<InteractionHub>(out hub)) return;
            
            packet.Set(shootDirection.Value);
            packet.Set(fireAnchor.Value);
            var identity = packet.Get<IIdentity>();
            
            gun.Begin(identity, target.Value, hub);
        }

        protected override void OnUpdate(Packet packet)
        {
            if (shootDirection.IsValid(packet) && fireAnchor.IsValid(packet) && target.IsValid(packet))
            {
                packet.Set(shootDirection.Value);
                var identity = packet.Get<IIdentity>();
                
                if (!gun.Use(identity, target.Value, hub)) return;

                gun.End(identity, target.Value, hub);
                isDone = true;
            }
            else isDone = true;
        }
    }
}