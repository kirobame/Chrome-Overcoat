using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveControl : MonoBehaviour
    {
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

        void Awake() => speedModifier = 1.0f;
        
        void Update()
        {
            var speed = this.speed * speedModifier;
            float ratio;

            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;

            if (body.IsGrounded)
            {
                if (Inputs != Vector3.zero && !IsWalking) IsWalking = SetAnimationState(WALK_STATE, true);
                else if (Inputs == Vector3.zero && IsWalking) IsWalking = SetAnimationState(WALK_STATE, false);
                
                if (Input.GetKey(KeyCode.LeftShift) && Inputs.z > 0)
                {
                    if (!IsSprinting) IsSprinting = SetAnimationState(RUN_STATE, true);
                    speed += sprint;
                }
                else if (IsSprinting) IsSprinting = SetAnimationState(RUN_STATE, false);
                
                airFriction.SetTimer(0.0f);
                ratio = 0.0f;
            }
            else
            {
                if (IsWalking) IsWalking = SetAnimationState(WALK_STATE, false);
                if (IsSprinting) IsSprinting = SetAnimationState(RUN_STATE, false);
                
                ratio = airFriction.Compute();
            }
            
            Direction = body.transform.TransformVector(Inputs);
            var delta = Direction * speed;
            
            groundDelta = Vector3.SmoothDamp(groundDelta, delta * (1.0f - ratio), ref groundVelocity, smoothing);
            body.intent += groundDelta;

            airDelta = Vector3.SmoothDamp(airDelta, delta * ratio, ref airVelocity, smoothing * airManoeuvrability);
            body.velocity += airDelta * Time.deltaTime;
        }

        private bool SetAnimationState(string name, bool value)
        {
            animator.SetBool(name, value);
            return value;
        }
    }
}