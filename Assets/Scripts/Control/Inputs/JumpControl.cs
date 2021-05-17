using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class JumpControl : InputControl<JumpControl>
    {
        protected override void PrepareInjection()
        {
            body = injections.Register(new AnyValue<CharacterBody>());
            gravity = injections.Register(new AnyValue<Gravity>());
        }
        
        protected override void SetupInputs()
        {
            key = new CachedValue<Key>(Key.Inactive);
            input.Value.BindKey(InputRefs.JUMP, this, key);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private float margin;
        [FoldoutGroup("Values"), SerializeField] private float height;
        [FoldoutGroup("Values"), SerializeField] private float pressThreshold;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        
        private float pressTime;
        private float error;

        private CachedValue<Key> key;
        
        void Update()
        {
            if (key.IsOn()) pressTime += Time.deltaTime;

            if (body.Value.IsGrounded) error = 0.0f;
            else error += Time.deltaTime;
            
            if (!key.IsUp()) return;
            
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