using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JumpHandler : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        [BoxGroup("Dependencies"), SerializeField] private MoveHandler move;
        
        [BoxGroup("Values"), SerializeField] private float margin;
        [BoxGroup("Values"), SerializeField] private float height;
        [BoxGroup("Values"), SerializeField] private float keyDownDetection;

        private float pressTime;
        private float error;
        
        void Update()
        {
            error += Time.deltaTime * (body.IsGrounded ? -1.0f : 1.0f);
            error = Mathf.Clamp(error, 0.0f, margin);
            
            if (Input.GetKey(KeyCode.Space)) pressTime += Time.deltaTime;
            if (!Input.GetKeyUp(KeyCode.Space)) return;
            
            if (error >= margin)
            {
                EndInput();
                return;
            }

            if (pressTime <= keyDownDetection)
            {
                var length = Mathf.Sqrt(2.0f * height * Physics.gravity.magnitude);
                body.velocity += Vector3.up * length;
            }
                
            EndInput();
        }
        
        private void EndInput()
        {
            pressTime = 0.0f;
            error = margin;
        }
    }
}