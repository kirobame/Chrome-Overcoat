using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class AirControl : InputControl
    {
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;

        [FoldoutGroup("Values"), SerializeField] private float speed;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private Vector3 inputs;
        private Vector3 damping;
        
        void Update()
        {
            if (body.IsGrounded) return;
            
            var rawInputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            inputs = Vector3.SmoothDamp(inputs, rawInputs, ref damping, smoothing);
            var direction = body.transform.TransformVector(inputs);

            body.force += direction * speed;
        }
    }
}