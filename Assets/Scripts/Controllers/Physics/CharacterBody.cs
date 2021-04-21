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
        
        protected override Vector3 Move(Vector3 delta)
        {
            hit = null;
            
            controller.Move(delta);
            return controller.velocity;
        }

        protected override CollisionHit HandleCollisions()
        {
            if (hit == null) return null;

            if (IsGrounded)
            {
                velocity = Vector3.zero;
                //Debug.Log("GROUNDED");
            }
            return new CollisionHit(this, hit.collider, hit.point, hit.normal, Delta);
        }

        void OnControllerColliderHit(ControllerColliderHit hit) => this.hit = hit;
    }
}