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
        
        [FoldoutGroup("Values"), SerializeField] private float cooldown;
        [FoldoutGroup("Values"), SerializeField] private Vector2 pressRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 heightRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 forwards;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private float shakeFactor;
        [FoldoutGroup("Feedbacks"), SerializeField] private float maxShake;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        private IValue<MoveControl> move;
        
        //private JetpackHUD HUD;
        private UIValue jetpackValues;
        
        private Coroutine cooldownRoutine;
        private float pressTime;
        private CachedValue<Key> key;

        //--------------------------------------------------------------------------------------------------------------/

        void Start()
        {
            jetpackValues = Repository.Get<UIValue>(UIValuesReferences.Jetpack);
            //HUD = Repository.Get<JetpackHUD>(Interface.Jetpack);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (body.Value.IsGrounded)
            {
                if (key.IsOn() && cooldownRoutine == null)
                {
                    pressTime += Time.deltaTime;
                    var ratio = Mathf.InverseLerp(pressRange.x, pressRange.y, pressTime);

                    jetpackValues.Set(ratio, "CHARGE");
                    //HUD.IndicateCharge(ratio);

                    Events.ZipCall(PlayerEvent.OnShake, ratio * shakeFactor, maxShake);
                }
            }

            if (!key.IsUp() || cooldownRoutine != null) return;
            
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

            jetpackValues.Set(0.0f, "CHARGE");
            //HUD.IndicateCharge(0.0f);
        }
        
        private IEnumerator CooldownRoutine()
        {
            var time = cooldown;

            while (time > 0.0f)
            {
                jetpackValues.Set(new System.Collections.Generic.KeyValuePair<float, float>(time, cooldown), "COOLDOWN");
                //HUD.IndicateCooldown(time, cooldown);
                
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }

            jetpackValues.Set(new System.Collections.Generic.KeyValuePair<float, float>(0.0f, cooldown), "COOLDOWN");
            //HUD.IndicateCooldown(0.0f, cooldown);
            cooldownRoutine = null;
        }
    }
}