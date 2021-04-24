using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class LinearMove : Move
    {
        public LinearMove(float speed, float radius, IValue<Vector3> direction, IValue<HashSet<Collider>> ignores, IValue<Transform> self) : base(self)
        {
            this.speed = speed;
            this.radius = radius;

            this.direction = direction;
            this.ignores = ignores;
        }

        private IValue<Vector3> direction;
        private IValue<HashSet<Collider>> ignores;

        private float radius;
        private float speed;
        
        protected override bool Execute(Packet packet, Transform self, out CollisionHit<Transform> hit)
        {
            if (!direction.IsValid(packet) || !ignores.IsValid(packet))
            {
                hit = null;
                return false;
            }

            var length = speed * Time.deltaTime;
            var ray = new Ray(self.position + direction.Value * radius, direction.Value);
            var delta = direction.Value * length;

            if (Physics.Raycast(ray, out var rayHit, length, LayerMask.GetMask("Environment", "Entity")) && !ignores.Value.Contains(rayHit.collider))
            {
                hit = new CollisionHit<Transform>(self, rayHit.collider, rayHit.point, rayHit.normal, delta);
                return true;
            }
            else
            {
                self.position += delta;
                self.rotation = Quaternion.LookRotation(direction.Value);

                hit = null;
                return false;
            }
        }

        
    }
}