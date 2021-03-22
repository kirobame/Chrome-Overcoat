using System;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct PhysicState : IData, IBridge<CharacterController>
    {
        public bool IsGrounded => isGrounded;

        [Range(0.0f, 1.0f)] public float friction;
        [HideInInspector] public Vector3 intent;
        [HideInInspector] public Vector3 velocity;
        
        private bool isGrounded;

        public void ReceiveDataFrom(CharacterController component) => isGrounded = component.isGrounded;
        public void SendDataTo(CharacterController component)
        {
            var delta = intent + velocity;
            component.Move(delta);
            
            intent = Vector3.zero;
        }
    }
}