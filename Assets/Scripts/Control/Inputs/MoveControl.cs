﻿using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveControl : InputControl<MoveControl>, IInjectable, IInjectionCallbackListener
    {
        private const string WALK_STATE = "Walk";
        private const string SPRINT_STATE = "Run";

        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        void IInjectionCallbackListener.OnInjectionDone(IRoot source) => body.Value.onCollision += OnBodyCollision;

        //--------------------------------------------------------------------------------------------------------------/

        public ModifiableFloat Speed => speed;

        public bool IsSprinting
        {
            get => isSprinting;
            set
            {
                isSprinting = value;
                animator.SetBool(SPRINT_STATE, value);
                
                if (value) Events.ZipCall(GaugeEvent.OnSprint, (byte)0);
                else Events.ZipCall(GaugeEvent.OnSprint, (byte)2);
            }
        }
        private bool isSprinting;
        
        public bool IsWalking     
        {
            get => isWalking;
            set
            {
                isWalking = value;
                animator.SetBool(WALK_STATE, value);
                
                if (value) Events.ZipCall(GaugeEvent.OnGroundMove, (byte)0);
                else Events.ZipCall(GaugeEvent.OnGroundMove, (byte)2);
            }
        }
        private bool isWalking;
        
        public Vector3 Direction { get; private set; }
        public Vector3 Inputs { get; private set; }
        
        [FoldoutGroup("Values"), SerializeField] private ModifiableFloat speed;
        [FoldoutGroup("Values"), SerializeField] private float sprint;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        [FoldoutGroup("Feedbacks"), SerializeField] private Animator animator;

        private IValue<IIdentity> identity;
        private IValue<CharacterBody> body;
        
        private Vector3 planeNormal;
        private Vector3 smoothedInputs;
        private Vector3 damping;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            body = new AnyValue<CharacterBody>();
            injections = new IValue[]
            {
                identity,
                body
            };
            
            speed.Bootup();
            speed.Modify(new Spring(0.075f));
        }
        void OnDestroy() => body.Value.onCollision -= OnBodyCollision;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (IsSprinting) Events.ZipCall(GaugeEvent.OnSprint, (byte)1);
            if (IsWalking) Events.ZipCall(GaugeEvent.OnGroundMove, (byte)1);

            if (!body.Value.IsGrounded)
            {
                smoothedInputs = Vector3.zero;
                
                if (IsSprinting) IsSprinting = false;
                if (IsWalking) IsWalking = false;

                return;
            }
           
            var speed = this.speed.Value;
            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
                
            if (Inputs != Vector3.zero && !IsWalking) IsWalking = true;
            else if (Inputs == Vector3.zero && IsWalking) IsWalking = false;
            
            if (Input.GetKey(KeyCode.LeftShift) && CanSprint() && Inputs.z > 0)
            {
                if (!IsSprinting) IsSprinting = true;
                speed += sprint;
            }
            else if (IsSprinting) IsSprinting = false;
                
            smoothedInputs = Vector3.SmoothDamp(smoothedInputs, Inputs, ref damping, smoothing);
            if (smoothedInputs.magnitude < 0.001f) return;

            Direction = body.Value.transform.TransformVector(smoothedInputs);
            var slopedDirection = Vector3.ProjectOnPlane(Direction, planeNormal);
            if (slopedDirection.y < 0) Direction = Vector3.Normalize(slopedDirection) * Direction.magnitude;
            
            var planarDelta = Vector3.ProjectOnPlane(body.Value.Velocity, planeNormal);
            if (planarDelta.magnitude > speed) planarDelta = planarDelta.normalized * speed;
            
            var delta = Direction * speed - planarDelta;
            if (delta.magnitude > speed) delta = delta.normalized * speed;
            
            body.Value.velocity += delta;
        }

        void OnBodyCollision(CollisionHit<PhysicBody> hit)
        {
            if (!body.Value.IsGrounded) return;
            planeNormal = hit.Normal;
        }

        private bool CanSprint()
        {
            var board = identity.Value.Packet.Get<IBlackboard>();
            return board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).Value;
        }
    }
}