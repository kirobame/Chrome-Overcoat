using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveControl : MonoBehaviour
    {
        public static bool canSprint;
        public static bool canShowWalk;
        
        private const string WALK_STATE = "Walk";
        private const string RUN_STATE = "Run";
        
        public Gain AirFriction => airFriction;
        
        public bool IsWalking { get; private set; }
        public bool IsSprinting { get; private set; }
        
        public Vector3 Inputs { get; private set; }
        public Vector3 Direction { get; private set; }
        
        public PhysicBody Body => body;
        
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;

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

        void Awake()
        {
            canSprint = true;
            canShowWalk = true;
            speedModifier = 1.0f;

            hardAssign = false;
            body.onCollision += OnBodyHit;
        }
        void OnDestroy() => body.onCollision -= OnBodyHit;

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
            
            body.intent += groundDelta;
            body.velocity += airDelta * Time.deltaTime;
            
            if (IsWalking && canShowWalk) animator.SetBool(WALK_STATE, true);
            else if (IsWalking && !canShowWalk) animator.SetBool(WALK_STATE, false);
            
            if (!canSprint) animator.SetBool(RUN_STATE, false);
        }

        private bool SetAnimationState(string name, bool value)
        {
            if (canShowWalk) animator.SetBool(name, value);
            return value;
        }

        void OnBodyHit(ControllerColliderHit hit)
        {
            if (hit.normal.y < 0) return;

            var projection = Vector3.Project(hit.normal, Vector3.up);
            var angle = Vector3.Angle(projection, hit.normal);

            if (angle > 1.0f && angle <= body.Controller.slopeLimit) hardAssign = true;
        }
    }
}