using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class CharacterBody : PhysicBody
    {
        public bool IsGrounded => controller.isGrounded;
        
        public override Collider Collider => controller;
        public CharacterController Controller => controller;
        
        [BoxGroup("Dependencies", Order = -1), SerializeField] private CharacterController controller;

        private ControllerColliderHit hit;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override Vector3 Move(Vector3 delta)
        {
            hit = null;
            
            controller.Move(delta);
            return controller.velocity;
        }

        protected override CollisionHit<PhysicBody> HandleCollisions()
        {
            if (hit == null) return null;

            if (IsGrounded) velocity = Vector3.zero;
            else
            {
                var penetration = velocity;
                if (Vector3.Dot(penetration.normalized, -hit.normal) > 0)
                {
                    var projection = Vector3.ProjectOnPlane(penetration, hit.normal);
                    velocity += projection - penetration;
                }
            }
            
            return new CollisionHit<PhysicBody>(this, hit.collider, hit.point, hit.normal, Delta);
        }

        void OnControllerColliderHit(ControllerColliderHit hit) => this.hit = hit;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void InstallDependenciesOn(Packet packet)
        {
            base.InstallDependenciesOn(packet);
            
            packet.Set(this);
            packet.Set(controller);
        }
    }
}