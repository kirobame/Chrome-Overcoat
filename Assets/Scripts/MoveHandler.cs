using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveHandler : MonoBehaviour
    {
        public Vector3 Intent { get; private set; }

        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        
        [BoxGroup("Values"), SerializeField] private float speed;
        [BoxGroup("Values"), SerializeField] private float smoothing;
        [BoxGroup("Values"), SerializeField, Range(0.0001f, 1.0f)] private float airControl;

        [BoxGroup("Sprint"), SerializeField] private AnimationCurve acceleration;
        [BoxGroup("Sprint"), SerializeField] private float boost;
        [BoxGroup("Sprint"), SerializeField] private float peek;
        [BoxGroup("Sprint"), SerializeField] private Vector2 rate;
        
        private bool isSprinting;
        private float sprintTime;
        
        private Vector3 velocity;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && body.Controller.isGrounded) isSprinting = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) isSprinting = false;
            
            var speed = this.speed;
            if (isSprinting)
            {
                if (body.IsGrounded) sprintTime += Time.deltaTime * rate.x;
                else
                {
                    isSprinting = false;
                    sprintTime -= Time.deltaTime * rate.y;
                }
            }
            else sprintTime -= Time.deltaTime * rate.y;

            sprintTime = Mathf.Clamp(sprintTime, 0.0f, peek);
            speed += acceleration.Evaluate(sprintTime / peek) * boost;
            
            var inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            var direction = body.transform.TransformVector(inputs);

            Intent = Vector3.SmoothDamp(Intent, direction * speed, ref velocity, body.IsGrounded ? smoothing : smoothing / airControl);
            body.intent += Intent;
        }
    }
}