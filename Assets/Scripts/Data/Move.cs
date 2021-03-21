using System;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public struct Move : IData, IWriter<CharacterController>
    {
        public Vector3 velocity;

        [SerializeField] private Transform reference;
        
        public void SendDataTo(CharacterController component)
        {
            var forward = reference.forward;
            forward.y = 0;
            
            forward.Normalize();
            velocity = Quaternion.identity * Quaternion.LookRotation(forward) * velocity;
            
            component.Move(velocity);
        }
    }
}