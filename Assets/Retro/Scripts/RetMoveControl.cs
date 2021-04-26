using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetMoveControl : InputControl, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        protected IIdentity identity;
        
        public Vector3 Inputs { get; private set; }
        
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterBody body;

        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private Vector3 planeNormal;
        private Vector3 smoothedInputs;
        private Vector3 damping;
        
        void Awake() => body.onCollision += OnBodyCollision;
        void OnDestroy() => body.onCollision -= OnBodyCollision;
        
        void Update()
        {
            if (!body.IsGrounded)
            {
                smoothedInputs = Vector3.zero;
                return;
            }
            
            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            smoothedInputs = Vector3.SmoothDamp(smoothedInputs, Inputs, ref damping, smoothing);
            if (smoothedInputs.magnitude < 0.001f) return;
            
            var slopedDirection = Vector3.ProjectOnPlane(smoothedInputs, planeNormal);
            if (slopedDirection.y < 0)
            {
                Debug.Log("A");
                smoothedInputs = slopedDirection * smoothedInputs.magnitude;
            }
            
            var planarDelta = Vector3.ProjectOnPlane(body.Velocity, planeNormal);
            if (planarDelta.magnitude > speed)
            {
                Debug.Log("B");
                planarDelta = planarDelta.normalized * speed;
            }
            
            var delta = smoothedInputs * speed - planarDelta;
            if (delta.magnitude > speed) delta = delta.normalized * speed;
            
            Debug.Log(delta.magnitude);
            body.velocity += delta;
        }

        void OnBodyCollision(CollisionHit<PhysicBody> hit)
        {
            if (!body.IsGrounded) return;
            planeNormal = hit.Normal;
        }
    }
}