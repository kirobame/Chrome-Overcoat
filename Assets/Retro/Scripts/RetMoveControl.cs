using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetMoveControl : InputControl
    {
        public Vector3 Inputs { get; private set; }
        
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterBody body;
        [FoldoutGroup("Dependencies"), SerializeField] private Gravity gravity;

        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private Vector3 planeNormal;
        private Vector3 smoothedInputs;
        private Vector3 damping;
        
        void Awake() => body.onCollision += OnBodyCollision;
        void OnDestroy() => body.onCollision -= OnBodyCollision;
        
        void FixedUpdate()
        {
            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            smoothedInputs = Vector3.SmoothDamp(smoothedInputs, Inputs, ref damping, smoothing);
            
            if (!body.IsGrounded || smoothedInputs.magnitude < 0.001f) return;

            var direction = smoothedInputs;
            var slopedDirection = Vector3.ProjectOnPlane(smoothedInputs, planeNormal);
            if (Vector3.Dot(gravity.Value.normalized, slopedDirection) > 0) direction = slopedDirection.normalized * smoothedInputs.magnitude;
            
            var planarDelta = Vector3.ProjectOnPlane(body.Velocity, planeNormal);
            if (planarDelta.magnitude > speed) planarDelta = planarDelta.normalized * speed;

            var delta = direction * speed - planarDelta;
            if (delta.magnitude > speed) delta = delta.normalized * speed;

            body.velocity += delta;
        }

        void OnBodyCollision(CollisionHit<PhysicBody> hit)
        {
            if (!body.IsGrounded) return;
            planeNormal = hit.Normal;
        }
    }
}