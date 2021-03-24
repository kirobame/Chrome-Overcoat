using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class DebugHandler : MonoBehaviour
    {
        
    }

    public class JumpHandler : MonoBehaviour
    {
        
    }
    
    public class MoveHandler : MonoBehaviour
    {
        [SerializeField] private PhysicBody body;
        
        [BoxGroup("Move"), SerializeField] private float speed;
        [BoxGroup("Move"), SerializeField] private float sprintSpeed;
        [BoxGroup("Move"), SerializeField] private float smoothing;

        [BoxGroup("Jump"), SerializeField] private float jumpForce;

        private bool isSprinting;
        
        private Vector3 current;
        private Vector3 velocity;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && body.Controller.isGrounded) isSprinting = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) isSprinting = false;
            
            if (Input.GetKeyDown(KeyCode.Space) && body.Controller.isGrounded) Jump();
            Move();
        }
        
        private void Move()
        {
            var boost = speed;
            if (isSprinting)
            {
                isSprinting = body.Controller.isGrounded;
                boost += sprintSpeed;
            }
            
            var target = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            target = body.transform.TransformVector(target.normalized) * boost;

            current = Vector3.SmoothDamp(current, target, ref velocity, smoothing);
            body.intent += current;
        }
        private void Jump()
        {
            var delta = Vector3.up * jumpForce;
            body.velocity += delta;
        }
    }
}