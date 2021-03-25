using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveControl : MonoBehaviour
    {
        public Gain AirFriction => airFriction;
        
        public Vector3 Inputs { get; private set; }
        public Vector3 Direction { get; private set; }
        
        public PhysicBody Body => body;
        
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;

        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float sprint;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        [FoldoutGroup("Airborne"), SerializeField] private Gain airFriction;
        [FoldoutGroup("Airborne"), SerializeField, Range(0.001f, 1.0f)] private float airManoeuvrability;

        private Vector3 groundDelta;
        private Vector3 groundVelocity;
        
        private Vector3 airDelta;
        private Vector3 airVelocity;

        void Update()
        {
            var speed = this.speed;
            float ratio;

            if (body.IsGrounded)
            {
                if (Input.GetKey(KeyCode.LeftShift)) speed += sprint;
                
                airFriction.SetTimer(0.0f);
                ratio = 0.0f;
            }
            else ratio = airFriction.Compute();

            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            Direction = body.transform.TransformVector(Inputs);

            var delta = Direction * speed;

            groundDelta = Vector3.SmoothDamp(groundDelta, delta, ref groundVelocity, smoothing);
            body.move += groundDelta;

            airDelta = Vector3.SmoothDamp(airDelta, delta * ratio, ref airVelocity, smoothing * airManoeuvrability);
            //body.intent += airDelta;
        }
    }
}