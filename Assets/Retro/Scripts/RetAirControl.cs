﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetAirControl : InputControl
    {
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterBody body;
        [FoldoutGroup("Dependencies"), SerializeField] private Lifetime life;
        
        [FoldoutGroup("Values"), SerializeField] private float maxSpeed;
        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private Vector3 smoothedInputs;
        private Vector3 damping;

        void FixedUpdate()
        {
            if (transform.position.y < -5.0f)
            {
                life.End();
                return;
            }
            
            if (body.IsGrounded)
            {
                smoothedInputs = Vector3.zero;
                return;
            }
            
            var inputs = RetExtensions.GetMoveInput().normalized;
            smoothedInputs = Vector3.SmoothDamp(smoothedInputs, inputs, ref damping, smoothing);
            var direction = smoothedInputs;

            var planarDelta = Vector3.ProjectOnPlane(body.Delta, Vector3.up);
            if (planarDelta.magnitude > maxSpeed) planarDelta = planarDelta.normalized * maxSpeed;

            var delta = direction * speed;
            var total = planarDelta + delta;
            if (total.magnitude > maxSpeed) delta = (planarDelta + delta).normalized * maxSpeed - planarDelta;
            
            body.velocity += delta;
        }
    }
}