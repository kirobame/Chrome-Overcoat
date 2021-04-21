using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveControl : InputControl
    {
        private const string WALK_STATE = "Walk";
        private const string SPRINT_STATE = "Run";
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public ModifiableFloat Speed => speed;

        public bool IsSprinting
        {
            get => isSprinting;
            set
            {
                isSprinting = value;
                animator.SetBool(SPRINT_STATE, value);
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
            }
        }
        private bool isWalking;
        
        [BoxGroup("Dependencies"), SerializeField] private PlayerBoard board;
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;
        
        [FoldoutGroup("Values"), SerializeField] private ModifiableFloat speed;
        [FoldoutGroup("Values"), SerializeField] private float sprint;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        
        [FoldoutGroup("ControlLoss"), SerializeField] private AnimationCurve airMap;
        [FoldoutGroup("ControlLoss"), SerializeField] private float airTime;
        [FoldoutGroup("ControlLoss"), SerializeField] private Vector2 airRates;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private Animator animator;
        
        private Vector3 planeNormal;
        private Vector3 inputs;
        private Vector3 damping;

        private float airTimer;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            speed.Bootup();
            speed.Modify(new Spring(0.075f));
            
            body.onCollision += OnBodyCollision;
        }
        void OnDestroy() => body.onCollision -= OnBodyCollision;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            var speed = this.speed.Value;
            var rawInputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;

            if (!body.IsGrounded)
            {
                airTimer += Time.deltaTime * airRates.x;
                
                IsSprinting = false;
                IsWalking = false;
                
                planeNormal = Vector3.up;
            }
            else
            {
                if (rawInputs != Vector3.zero && !IsWalking) IsWalking = true;
                else if (rawInputs == Vector3.zero && IsWalking) IsWalking = false;
                
                if (Input.GetKey(KeyCode.LeftShift) && board.Get<BusyBool>("canSprint").Value && rawInputs.z > 0)
                {
                    if (!IsSprinting) IsSprinting = true;
                    speed += sprint;
                }
                else if (IsSprinting) IsSprinting = false;
                
                airTimer -= Time.deltaTime * airRates.x;
            }

            airTimer = Mathf.Clamp(airTimer, 0.0f, airTime);
            speed *= airMap.Evaluate(airTimer / airTime);
            
            inputs = Vector3.SmoothDamp(inputs, rawInputs, ref damping, smoothing);
            if (inputs.magnitude < 0.001f) return;

            var direction = body.transform.TransformVector(inputs);
            var slopedDirection = Vector3.ProjectOnPlane(direction, planeNormal);
            if (slopedDirection.y < 0) direction = Vector3.Normalize(slopedDirection) * direction.magnitude;

            var planarDelta = Vector3.ProjectOnPlane(body.velocity, planeNormal);
            if (planarDelta.magnitude > speed) planarDelta = planarDelta.normalized * speed;
            var delta = direction * speed - planarDelta;
            
            body.velocity += delta;
        }

        void OnBodyCollision(CollisionHit hit)
        {
            if (!body.IsGrounded) return;
            planeNormal = hit.Normal;
        }
        
        /*public static bool canSprint;
        public static bool canShowWalk;
        
        private const string WALK_STATE = "Walk";
        private const string RUN_STATE = "Run";
        
        public Gain AirFriction => airFriction;
        
        public bool IsWalking { get; private set; }
        public bool IsSprinting { get; private set; }
        
        public Vector3 Inputs { get; private set; }
        public Vector3 Direction { get; private set; }
        
        public PhysicBody Body => body;
        
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;

        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float sprint;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        [FoldoutGroup("Airborne"), SerializeField] private Gain airFriction;
        [FoldoutGroup("Airborne"), SerializeField, Range(0.001f, 1.0f)] private float airManoeuvrability;

        [FoldoutGroup("Feedbacks"), SerializeField] private Animator animator;

        [HideInInspector] public float speedModifier;
        
        private Vector3 groundDelta;
        private Vector3 groundVelocity;
        
        private Vector3 airDelta;
        private Vector3 airVelocity;

        private bool hardAssign;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            Bootup();

            hardAssign = false;
            body.onCollision += OnBodyHit;
        }
        void OnDestroy() => body.onCollision -= OnBodyHit;

        public override void Bootup()
        {
            canSprint = true;
            canShowWalk = true;
            speedModifier = 1.0f;
            
            base.Bootup();
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;
            
            canSprint = true;
            canShowWalk = true;
        }

        void Update()
        {
            var speed = this.speed * speedModifier;
            float ratio;

            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            Direction = body.transform.TransformVector(Inputs);
            
            if (body.IsGrounded)
            {
                if (Inputs != Vector3.zero && !IsWalking) IsWalking = SetAnimationState(WALK_STATE, true);
                else if (Inputs == Vector3.zero && IsWalking) IsWalking = SetAnimationState(WALK_STATE, false);
                
                if (Input.GetKey(KeyCode.LeftShift) && canSprint && Inputs.z > 0)
                {
                    if (!IsSprinting) IsSprinting = SetAnimationState(RUN_STATE, true);
                    speed += sprint;
                }
                else if (IsSprinting) IsSprinting = SetAnimationState(RUN_STATE, false);
                
                airFriction.SetTimer(0.0f);
                ratio = 0.0f;

                var ray = new Ray(transform.position + Vector3.up * body.Controller.skinWidth, Vector3.down);
                if (Physics.Raycast(ray, out var hit, LayerMask.GetMask("Environment")))
                {
                    if (Vector3.Dot(Direction, hit.normal) > 0.0f)
                    {
                        Direction = (Vector3.ProjectOnPlane(Direction, hit.normal) + Vector3.down * (body.Controller.skinWidth * 2.0f)).normalized;
                        body.velocity = Vector3.zero;
                    }
                }
            }
            else
            {
                if (IsWalking) IsWalking = SetAnimationState(WALK_STATE, false);
                if (IsSprinting) IsSprinting = SetAnimationState(RUN_STATE, false);
                
                ratio = airFriction.Compute();
            }
            
            var delta = Direction * speed;

            if (!hardAssign)
            {
                groundDelta = Vector3.SmoothDamp(groundDelta, delta * (1.0f - ratio), ref groundVelocity, smoothing);
                airDelta = Vector3.SmoothDamp(airDelta, delta * ratio, ref airVelocity, smoothing * airManoeuvrability);
            }
            else
            {
                groundDelta = delta * (1.0f - ratio);
                airDelta = delta * ratio;

                hardAssign = false;
            }
            
            if (body.IsGrounded) body.displacement += groundDelta;
            else body.force += airDelta;
            
            if (IsWalking && canShowWalk) animator.SetBool(WALK_STATE, true);
            else if (IsWalking && !canShowWalk) animator.SetBool(WALK_STATE, false);
            
            if (!canSprint) animator.SetBool(RUN_STATE, false);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private bool SetAnimationState(string name, bool value)
        {
            if (canShowWalk) animator.SetBool(name, value);
            return value;
        }

        void OnBodyHit(CollisionHit hit)
        {
            if (hit.Normal.y < 0) return;

            var projection = Vector3.Project(hit.Normal, Vector3.up);
            var angle = Vector3.Angle(projection, hit.Normal);

            if (angle > 1.0f && angle <= body.Controller.slopeLimit) hardAssign = true;
        }*/
    }
}