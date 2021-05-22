using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flux;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class JetpackControl : InputControl<JetpackControl>
    {
        protected override void PrepareInjection()
        {
            body = injections.Register(new AnyValue<CharacterBody>());
            gravity = injections.Register(new AnyValue<Gravity>());
            move = injections.Register(new AnyValue<MoveControl>());
        }
        
        protected override void SetupInputs()
        {
            key = new CachedValue<Key>(Key.Inactive);
            input.Value.BindKey(InputRefs.JUMP, this, key);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private BindableCooldown cooldown;
        [FoldoutGroup("Values"), SerializeField] private BindableRatio charge;
        [FoldoutGroup("Values"), SerializeField] private Vector2 heightRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 forwards;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private float shakeFactor;
        [FoldoutGroup("Feedbacks"), SerializeField] private float maxShake;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        private IValue<MoveControl> move;

        private CachedValue<Key> key;

        //--------------------------------------------------------------------------------------------------------------/

        void Start() => HUDBinder.Declare(HUDGroup.Jetpack, charge, cooldown);
        
        void Update()
        {
            if (body.Value.IsGrounded)
            {
                if (key.IsOn() && !cooldown.IsActive)
                {
                    charge.Value += Time.deltaTime;
                    Events.ZipCall(PlayerEvent.OnShake, charge.Compute() * shakeFactor, maxShake);
                }
            }

            if (!key.IsUp() || cooldown.IsActive) return;
            
            if (!charge.IsAtMin)
            {
                var attraction = gravity.Value.Force;
                
                var height = Mathf.Lerp(heightRange.x, heightRange.y, charge.Compute());
                var length = -Mathf.Sqrt(height * 2.0f * attraction.magnitude);
                
                var launch = attraction.normalized * length;

                var direction = body.Value.transform.TransformVector(move.Value.Inputs);
                if (move.Value.IsSprinting) launch += direction * forwards.y;
                else if (move.Value.IsWalking) launch += direction * forwards.x;

                body.Value.velocity += launch;
                cooldown.Start();
                
                Events.ZipCall(GaugeEvent.OnJetpackUsed, launch);
            }

            charge.Value = 0.0f;
        }
    }
}