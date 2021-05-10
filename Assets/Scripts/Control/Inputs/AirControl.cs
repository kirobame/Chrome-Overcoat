﻿using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class AirControl : InputControl<AirControl>
    {
        public bool IsMoving     
        {
            get => isMoving;
            set
            {
                isMoving = value;

                if (value) Events.ZipCall(GaugeEvent.OnAirMove, (byte)0);
                else Events.ZipCall(GaugeEvent.OnAirMove, (byte)2);
            }
        }
        private bool isMoving;
        
        public Vector3 Direction { get; private set; }
        public Vector3 Inputs { get; private set; }
        
        [FoldoutGroup("Values"), SerializeField] private float maxSpeed;
        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private IValue<CharacterBody> body;
        
        private Vector3 smoothedInputs;
        private Vector3 damping;

        protected override void Awake()
        {
            base.Awake();
            
            body = new AnyValue<CharacterBody>();
            injections.Add(body);
        }
        
        protected override void SetupInputs() => input.Value.Bind(InputRefs.MOVE, this, OnMoveInput);
        void OnMoveInput(InputAction.CallbackContext context, InputCallbackType type)
        {
            var inputs2D = context.ReadValue<Vector2>();
            Inputs = new Vector3(inputs2D.x, 0.0f, inputs2D.y);
        }

        void Update()
        {
            if (IsMoving) Events.ZipCall(GaugeEvent.OnAirMove, (byte)1);
            
            if (body.Value.IsGrounded)
            {
                if (!isMoving) IsMoving = false;
                smoothedInputs = Vector3.zero;
                
                return;
            }
            
            if (Inputs != Vector3.zero && !IsMoving) IsMoving = true;
            else if (Inputs == Vector3.zero && IsMoving) IsMoving = false;
            
            smoothedInputs = Vector3.SmoothDamp(smoothedInputs, Inputs, ref damping, smoothing);
            Direction = body.Value.transform.TransformVector(smoothedInputs);

            var planarDelta = Vector3.ProjectOnPlane(body.Value.Delta, Vector3.up);
            if (planarDelta.magnitude > maxSpeed) planarDelta = planarDelta.normalized * maxSpeed;

            var delta = Direction * speed;
            var total = planarDelta + delta;
            if (total.magnitude > maxSpeed) delta = (planarDelta + delta).normalized * maxSpeed - planarDelta;
            
            body.Value.velocity += delta;
        }
    }
}