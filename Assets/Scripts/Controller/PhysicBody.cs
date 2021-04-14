using System;
using System.Collections.Generic;
using System.Linq;
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
        
        [HideInInspector] public Vector3 intent;
        [HideInInspector] public Vector3 velocity;

        void Update()
        {
            var delta = ComputeDelta();
            controller.Move(delta);
            onMove?.Invoke(delta);
            
            intent = Vector3.zero;
            if (IsGrounded) velocity = Vector3.zero;
        }
        
        public Vector3 ComputeDelta() => (intent + velocity) * Time.deltaTime;
        
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var velocityDelta = velocity * Time.deltaTime;
            
            if (velocityDelta.y > 0 && hit.normal.y < 0)
            {
                var length = velocityDelta.magnitude;
                velocity += hit.normal * length;
                
                intent = Vector3.zero;
                velocity.y = 0;
            }

            onCollision?.Invoke(hit);
        }
    }
}