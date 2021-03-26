﻿using System.Collections;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class JetpackControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        [BoxGroup("Dependencies"), SerializeField] private Gravity gravity;
        [BoxGroup("Dependencies"), SerializeField] private MoveControl move;

        [FoldoutGroup("Values"), SerializeField] private float cooldown;
        [FoldoutGroup("Values"), SerializeField] private Vector2 pressRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 heightRange;
        [FoldoutGroup("Values"), SerializeField] private Vector2 forwards;
        
        [FoldoutGroup("Airborne"), SerializeField] private Vector3 controlLosses;
        [FoldoutGroup("Airborne"), SerializeField] private float airTime;
        [FoldoutGroup("Airborne"), SerializeField] private float airForce;
        
        private JetpackHUD HUD;

        private bool hasJumped;
        private Coroutine cooldownRoutine;
        
        private float pressTime;
        private float airTimer;

        void Start()
        {
            airTimer = airTime;
            HUD = Repository.Get<JetpackHUD>(Interface.Jetpack);
        }

        void Update()
        {
            if (body.IsGrounded)
            {
                if (Input.GetKey(KeyCode.Space) && !hasJumped && cooldownRoutine == null)
                {
                    pressTime += Time.deltaTime;
                    HUD.IndicateCharge(Mathf.InverseLerp(pressRange.x, pressRange.y, pressTime));
                }
                
                airTimer += Time.deltaTime;
                if (airTimer > airTime) airTimer = airTime;
                
                HUD.IndicateAirTime(airTimer);
            }
            else
            {
                if (hasJumped) hasJumped = false;
                
                if (Input.GetKey(KeyCode.Space) && airTimer > 0.0f)
                {
                    airTimer -= Time.deltaTime;
                    if (airTimer < 0.0f) airTimer = 0.0f;

                    var attraction = gravity.Value;
                    body.velocity -= attraction.normalized * ((attraction.magnitude + airForce) * Time.deltaTime);
                    HUD.IndicateAirTime(airTimer);
                }
            }
            
            if (!Input.GetKeyUp(KeyCode.Space) || hasJumped || cooldownRoutine != null) return;

            if (pressTime > pressRange.x)
            {
                var attraction = gravity.Value;

                var ratio = Mathf.InverseLerp(pressRange.x, pressRange.y, pressTime);
                var height = Mathf.Lerp(heightRange.x, heightRange.y, ratio);
                var length = -Mathf.Sqrt(height * 2.0f * attraction.magnitude);
                
                var launch = attraction.normalized * length;
                float controlLoss;

                if (move.IsSprinting)
                {
                    launch += move.Direction * forwards.y;
                    controlLoss = controlLosses.z;
                }
                else if (move)
                {
                    launch += move.Direction * forwards.x;
                    controlLoss = controlLosses.y;
                }
                else controlLoss = controlLosses.x;

                body.velocity += launch;
                hasJumped = true;

                Routines.Start(Routines.DoAfter(() => move.AirFriction.SetTimer(controlLoss), new YieldFrame()));
                cooldownRoutine = StartCoroutine(CooldownRoutine());
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