using System.Collections.Generic;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class ThrusterControl : InputControl<ThrusterControl>
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
        
        [FoldoutGroup("Values"), SerializeField] private float airTime;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve map;
        [FoldoutGroup("Values"), SerializeField] private Vector2 inputRange;
        [FoldoutGroup("Values"), SerializeField] private float speed;

        private IValue<CharacterBody> body;
        private IValue<Gravity> gravity;
        
        private float airTimer;
        private JetpackHUD HUD;

        private CachedValue<Key> key;

        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            airTimer = airTime;
            HUD = Repository.Get<JetpackHUD>(Interface.Jetpack);
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (body.Value.IsGrounded)
            {
                airTimer += Time.deltaTime;
                if (airTimer > airTime) airTimer = airTime;
            }
            else
            {
                if (airTimer > 0.0f)
                {
                    if (key.IsDown()) Events.ZipCall(GaugeEvent.OnThrusterUsed, (byte)0);
                    
                    if (key.IsOn())
                    {
                        Events.ZipCall(GaugeEvent.OnThrusterUsed, (byte)1);
                        
                        airTimer -= Time.deltaTime;
                        if (airTimer < 0.0f)
                        {
                            Events.ZipCall(GaugeEvent.OnThrusterUsed, (byte)2);
                            airTimer = 0.0f;
                        }

                        var normalizedGravity = gravity.Value.Force.normalized;
                        var force = Vector3.Project(body.Value.Delta, normalizedGravity);
                        var attraction = force.magnitude * -Vector3.Dot(force.normalized, normalizedGravity);
                    
                        float ratio;
                        if (attraction < 0) ratio = map.Evaluate(Mathf.InverseLerp(inputRange.x, 0.0f, attraction) - 1.0f);
                        else ratio = map.Evaluate(Mathf.InverseLerp(0.0f, inputRange.y, attraction));

                        var delta = -gravity.Value.Force.normalized * (gravity.Value.Force.magnitude + speed * ratio);
                        body.Value.force += delta;
                    }
                
                    if (key.IsUp()) Events.ZipCall(GaugeEvent.OnThrusterUsed, (byte)2);
                }
            }
            
            HUD.IndicateAirTime(airTimer);
        }
    }
}