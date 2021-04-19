using System;
using System.Collections;
using Flux;
using Flux.Data;
using Flux.Event;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Charge : ProxyNode
    {
        public Charge(float duration, float shakeFactor, float maxShake)
        {
            this.duration = duration;

            this.shakeFactor = shakeFactor;
            this.maxShake = maxShake;
        }

        [SerializeField] private float duration;
        [SerializeField] private float shakeFactor;
        [SerializeField] private float maxShake;

        private bool hasStarted;
        private float timer;
        
        protected override void OnStart(Packet packet)
        {
            hasStarted = false;
            timer = 0.0f;
        }

        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<Blackboard>();
            float charge;
            
            if (!hasStarted)
            {
                charge = board.Get<float>("charge");

                if (charge <= 0)
                {
                    hasStarted = true;
                    board.Set(true, "charge.isUsed");
                }
                else return;
            }
            
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, duration);
            charge = timer / duration;

            board.Set(charge, "charge");
            Events.ZipCall(PlayerEvent.OnShake, shakeFactor * charge, maxShake);
            
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.Set(charge);
        }
    }
}