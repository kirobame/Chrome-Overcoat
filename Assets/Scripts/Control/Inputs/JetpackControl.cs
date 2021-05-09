using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flux;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JetpackControl : InputControl<JetpackControl>, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private float cooldown;
        [FoldoutGroup("Values"), SerializeField] private Vector2 pressRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 heightRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 forwards;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private float shakeFactor;
        [FoldoutGroup("Feedbacks"), SerializeField] private float maxShake;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        private IValue<MoveControl> move;
        
        private JetpackHUD HUD;
        
        private Coroutine cooldownRoutine;
        private float pressTime;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            body = new AnyValue<CharacterBody>();
            gravity = new AnyValue<Gravity>();
            move = new AnyValue<MoveControl>();
            injections = new IValue[]
            {
                body, 
                gravity,
                move
            };
        }
        void Start() => HUD = Repository.Get<JetpackHUD>(Interface.Jetpack);
        
        void Update()
        {
            if (body.Value.IsGrounded)
            {
                if (Input.GetKey(KeyCode.Space) && cooldownRoutine == null)
                {
                    pressTime += Time.deltaTime;
                    var ratio = Mathf.InverseLerp(pressRange.x, pressRange.y, pressTime);
                    
                    HUD.IndicateCharge(ratio);
                    Events.ZipCall(PlayerEvent.OnShake, ratio * shakeFactor, maxShake);
                }
            }

            if (!Input.GetKeyUp(KeyCode.Space) || cooldownRoutine != null) return;
            
            if (pressTime > pressRange.x)
            {
                var attraction = gravity.Value.Force;

                var ratio = Mathf.InverseLerp(pressRange.x, pressRange.y, pressTime);
                var height = Mathf.Lerp(heightRange.x, heightRange.y, ratio);
                var length = -Mathf.Sqrt(height * 2.0f * attraction.magnitude);
                
                var launch = attraction.normalized * length;

                var direction = body.Value.transform.TransformVector(move.Value.Inputs);
                if (move.Value.IsSprinting) launch += direction * forwards.y;
                else if (move.Value.IsWalking) launch += direction * forwards.x;

                body.Value.velocity += launch;
                cooldownRoutine = Routines.Start(CooldownRoutine());
                
                Events.ZipCall(GaugeEvent.OnJetpackUsed, launch);
            }

            pressTime = 0.0f;
            HUD.IndicateCharge(0.0f);
        }
        
        private IEnumerator CooldownRoutine()
        {
            var time = cooldown;

            while (time > 0.0f)
            {
                HUD.IndicateCooldown(time, cooldown);
                
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }
            
            HUD.IndicateCooldown(0.0f, cooldown);
            cooldownRoutine = null;
        }
    }
}