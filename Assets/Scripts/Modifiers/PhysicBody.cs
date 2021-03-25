using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PhysicBody : MonoBehaviour
    {
        public event Action<ControllerColliderHit> onCollision;
        public event Action<Vector3> onMove;
        
        public bool IsGrounded => controller.isGrounded;
        public CharacterController Controller => controller;
        
        [BoxGroup("Dependencies"), SerializeField] private CharacterController controller;

        [HideInInspector] public Vector3 move;
        [HideInInspector] public Vector3 intent;
        [HideInInspector] public Vector3 velocity;

        void LateUpdate()
        {
            var delta = ComputeDelta();
            controller.Move(delta);
            onMove?.Invoke(delta);
            
            velocity += intent * Time.deltaTime;
            intent = Vector3.zero;
            move = Vector3.zero;

            if (IsGrounded) velocity = Vector3.zero;
        }
        
        public Vector3 ComputeDelta() => (move + intent + velocity) * Time.deltaTime;
        
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var partialDelta = (intent + velocity) * Time.deltaTime;
            if (partialDelta.y > 0 && hit.normal.y < 0)
            {
                var length = partialDelta.magnitude;
                velocity += hit.normal * length;
                
                intent = Vector3.zero;
                velocity.y = 0;
            }
            
            onCollision?.Invoke(hit);
        }
    }
}