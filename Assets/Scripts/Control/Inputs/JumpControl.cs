using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class JumpControl : InputControl<JumpControl>
    {
        [FoldoutGroup("Values"), SerializeField] private float margin;
        [FoldoutGroup("Values"), SerializeField] private float height;
        [FoldoutGroup("Values"), SerializeField] private float pressThreshold;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        
        private float pressTime;
        private float error;

        private Key key;

        protected override void Awake()
        {
            key = Key.Default;
            
            base.Awake();
            
            body = new AnyValue<CharacterBody>();
            injections.Add(body);
            
            gravity = new AnyValue<Gravity>();
            injections.Add(gravity);
        }
        
        protected override void SetupInputs() => input.Value.Bind(InputRefs.JUMP, this, OnJumpInput, true);
        void OnJumpInput(InputAction.CallbackContext context, InputCallbackType type)
        {
            Debug.Log($"Jumping : {type}");
            key.Update(type);
        }

        void Update()
        {
            if (key.State == KeyState.On) pressTime += Time.deltaTime;

            if (body.Value.IsGrounded) error = 0.0f;
            else error += Time.deltaTime;
            
            if (key.State != KeyState.Up) return;
            
            if (error <= margin && pressTime <= pressThreshold)
            {
                Events.Call(GaugeEvent.OnJump);
                
                var attraction = gravity.Value.Force;
                var length = -Mathf.Sqrt(height * 2.0f * attraction.magnitude);
                
                body.Value.velocity += attraction.normalized * length;
            }
            
            pressTime = 0.0f;
        }
    }
}