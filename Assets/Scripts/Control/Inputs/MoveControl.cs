using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class MoveControl : InputControl<MoveControl>
    {
        private const string WALK_STATE = "Walk";
        private const string SPRINT_STATE = "Run";

        //--------------------------------------------------------------------------------------------------------------/

        protected override void PrepareInjection()
        {
            speed.Bootup();
            speed.Modify(new Spring(0.075f));
            
            identity = injections.Register(new AnyValue<IIdentity>());
            body = injections.Register(new AnyValue<CharacterBody>());
        }
        protected override void OnInjectionDone(IRoot source) => body.Value.onCollision += OnBodyCollision;
       
        protected override void SetupInputs()
        { 
            sprintKey = new CachedValue<Key>(Key.Inactive);

            input.Value.Bind(InputRefs.MOVE, this, OnMoveInput);
            input.Value.BindKey(InputRefs.SPRINT, this, sprintKey);
        }
        void OnMoveInput(InputAction.CallbackContext context, InputCallbackType type)
        {
            var inputs2D = context.ReadValue<Vector2>();
            Inputs = new Vector3(inputs2D.x, 0.0f, inputs2D.y);
        }

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

        private CachedValue<Key> sprintKey;

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            body.Value.onCollision -= OnBodyCollision;
        }

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

            if (Inputs != Vector3.zero && !IsWalking) IsWalking = true;
            else if (Inputs == Vector3.zero && IsWalking) IsWalking = false;
            
            if (sprintKey.IsOn() && CanSprint() && Inputs.z > 0)
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