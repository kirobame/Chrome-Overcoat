using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JumpControl : InputControl
    {
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;
        [BoxGroup("Dependencies"), SerializeField] private Gravity gravity;
        
        [FoldoutGroup("Values"), SerializeField] private float margin;
        [FoldoutGroup("Values"), SerializeField] private float height;
        [FoldoutGroup("Values"), SerializeField] private float pressThreshold;

        private float pressTime;
        private float error;
        
        void Update()
        {
            if (Input.GetKey(KeyCode.Space)) pressTime += Time.deltaTime;

            if (body.IsGrounded) error = 0.0f;
            else error += Time.deltaTime;
            
            if (!Input.GetKeyUp(KeyCode.Space)) return;

            if (error <= margin && pressTime <= pressThreshold)
            {
                Events.Call(GaugeEvent.OnJump);
                
                var attraction = gravity.Value;
                var length = -Mathf.Sqrt(height * 2.0f * attraction.magnitude);
                
                body.velocity += attraction.normalized * length;
            }
            
            pressTime = 0.0f;
        }
    }
}