using System.Collections;
using Flux;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class Attack : TaskNode
    {
        private const string ACTION = "Attack";

        //--------------------------------------------------------------------------------------------------------------/

        public Attack(IValue<Collider> collider, IValue<Transform> from, float damage, Vector3 configuration, float delay, float cooldown, PoolableVfx hitVfxPrefab)
        {
            this.collider = collider;
            this.from = from;
            
            this.damage = damage;
            angle = configuration.x;
            length = configuration.y;
            resolution = Mathf.RoundToInt(configuration.z);
            
            this.delay = delay;
            this.cooldown = cooldown;

            this.hitVfxPrefab = hitVfxPrefab;
        }
        
        private IValue<Collider> collider;
        private IValue<Transform> from;
        
        private float damage;
        private float angle;
        private float length;
        private int resolution;
        private float delay;
        private float cooldown;
        
        private PoolableVfx hitVfxPrefab;

        private Coroutine routine;

        protected override void OnUse(Packet packet)
        {
            if (routine == null) routine = Routines.Start(Routine(packet));
            isDone = true;
        }

        private IEnumerator Routine(Packet packet)
        {
            if (packet.TryGet<Animator>(out var animator)) animator.SetTrigger(ACTION);
            yield return new WaitForSeconds(delay);
            
            if (collider.IsValid(packet) && collider.Value is CharacterController capsuleCollider && from.IsValid(packet))
            {
                var identity = packet.Get<IIdentity>();
                var mask = LayerMask.GetMask("Entity", "Environment");

                var center = from.Value.position;
                var startingDirection = Quaternion.AngleAxis(-angle * 0.5f, from.Value.up) * from.Value.forward;
                var step = angle / resolution;

                var match = false;
                var shortestDistance = float.PositiveInfinity;
                var cachedHit = default(RaycastHit);
                var hub = default(InteractionHub);

                for (var i = 0; i < resolution; i++)
                {
                    var direction = Quaternion.AngleAxis(step * i , from.Value.up) * startingDirection;
                    var ray = new Ray(center, direction);

                    if (!Physics.Raycast(ray, out var hit, length, mask)) continue;

                    if (hit.distance >= shortestDistance || !hit.collider.TryGetComponent<InteractionHub>(out var hitHub) || hitHub.Identity.Faction == identity.Faction) continue;
                    
                    match = true;
                    shortestDistance = hit.distance;

                    cachedHit = hit;
                    hub = hitHub;
                }

                if (match)
                {
                    var VfxPool = Repository.Get<VfxPool>(Pool.Impact);
                    var VfxInstance = VfxPool.RequestSingle(hitVfxPrefab);

                    VfxInstance.transform.position = cachedHit.point;
                    VfxInstance.Play();
                    
                    hub.RelayDamage(identity, damage);
                }
            }
            
            yield return new WaitForSeconds(cooldown - delay);
            routine = null;
        }
    }
}