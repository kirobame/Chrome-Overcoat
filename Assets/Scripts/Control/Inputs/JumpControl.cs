using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JumpControl : InputControl<JumpControl>, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private float margin;
        [FoldoutGroup("Values"), SerializeField] private float height;
        [FoldoutGroup("Values"), SerializeField] private float pressThreshold;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        
        private float pressTime;
        private float error;

        void Awake()
        {
            body = new AnyValue<CharacterBody>();
            gravity = new AnyValue<Gravity>();
            injections = new IValue[]
            {
                body, 
                gravity
            };
        }
        
        void Update()
        {
            if (Input.GetKey(KeyCode.Space)) pressTime += Time.deltaTime;

            if (body.Value.IsGrounded) error = 0.0f;
            else error += Time.deltaTime;
            
            if (!Input.GetKeyUp(KeyCode.Space)) return;

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