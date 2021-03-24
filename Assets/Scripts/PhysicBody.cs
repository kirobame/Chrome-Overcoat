using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PhysicBody : MonoBehaviour
    {
        public event Action<ControllerColliderHit> onCollision;
        
        public bool IsGrounded => controller.isGrounded;
        public CharacterController Controller => controller;
        
        [BoxGroup("Dependencies"), SerializeField] private CharacterController controller;

        [BoxGroup("Values"), SerializeField, Range(0.0f, 5.0f)] private float affect;

        [HideInInspector] public Vector3 intent;
        [HideInInspector] public Vector3 velocity;

        void Update()
        {
            velocity += Physics.gravity * (affect * Time.deltaTime);
            controller.Move((velocity + intent) * Time.deltaTime);

            velocity += intent * Time.deltaTime;
            intent = Vector3.zero;
            
            if (IsGrounded) velocity = Vector3.zero;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (velocity.y > 0 && hit.normal.y < 0)
            {
                Debug.Log("Redirection");
            
                var length = velocity.magnitude;
                velocity += hit.normal * length;
                
                velocity.y = 0;
            }
            
            onCollision?.Invoke(hit);
        }
    }
}