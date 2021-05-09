using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class AirControl : InputControl<AirControl>, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
        
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
        
        [FoldoutGroup("Values"), SerializeField] private float maxSpeed;
        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private IValue<CharacterBody> body;
        
        private Vector3 smoothedInputs;
        private Vector3 damping;

        void Awake()
        {
            body = new AnyValue<CharacterBody>();
            injections = new IValue[] { body };
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
            
            var inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            
            if (inputs != Vector3.zero && !IsMoving) IsMoving = true;
            else if (inputs == Vector3.zero && IsMoving) IsMoving = false;
            
            smoothedInputs = Vector3.SmoothDamp(smoothedInputs, inputs, ref damping, smoothing);
            var direction = body.Value.transform.TransformVector(smoothedInputs);

            var planarDelta = Vector3.ProjectOnPlane(body.Value.Delta, Vector3.up);
            if (planarDelta.magnitude > maxSpeed) planarDelta = planarDelta.normalized * maxSpeed;

            var delta = direction * speed;
            var total = planarDelta + delta;
            if (total.magnitude > maxSpeed) delta = (planarDelta + delta).normalized * maxSpeed - planarDelta;
            
            body.Value.velocity += delta;
        }
    }
}