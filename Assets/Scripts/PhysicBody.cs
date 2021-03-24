using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Chrome
{
    public class PhysicBody : MonoBehaviour
    {
        public event Action<ControllerColliderHit> onCollision;
        
        public bool IsGrounded => controller.isGrounded;
        public CharacterController Controller => controller;
        
        [SerializeField] private CharacterController controller;

        [HideInInspector] public Vector3 intent;
        [HideInInspector] public Vector3 velocity;

        private bool ignoreCollisions;

        void Update()
        {
            ignoreCollisions = true;
            controller.Move(intent * Time.deltaTime);
            intent = Vector3.zero;
            ignoreCollisions = false;

            velocity += Physics.gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            if (IsGrounded) velocity = Vector3.zero;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (ignoreCollisions) return;

            if (velocity.y > 0 && hit.normal.y < 0)
            {
                var length = velocity.magnitude;
                velocity += hit.normal * length;
                
                velocity.y = 0;
            }
            
            onCollision?.Invoke(hit);
        }
    }
}