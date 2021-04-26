using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetJumpControl : InputControl
    {
        [FoldoutGroup("Dependencies"), SerializeField] private CharacterBody body;
        [FoldoutGroup("Dependencies"), SerializeField] private Gravity gravity;
        
        [FoldoutGroup("Values"), SerializeField] private float margin;
        [FoldoutGroup("Values"), SerializeField] private float height;

        private bool hasJumped;
        private float error;
        
        void Update()
        {
            if (body.IsGrounded)
            {
                if (hasJumped) hasJumped = false;
                error = 0.0f;
            }
            else error += Time.deltaTime;
            
            if (!Input.GetKeyUp(KeyCode.Space) || error > margin || hasJumped) return;

            var attraction = gravity.Value;
            var length = -Mathf.Sqrt(height * 2.0f * attraction.magnitude);
            body.velocity += attraction.normalized * length;

            hasJumped = true;
        }
    }
}