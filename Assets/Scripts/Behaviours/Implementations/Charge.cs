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

        private Coroutine cooldownRoutine;
        private float timer;

        protected override void OnStart(Packet packet)
        {
            if (cooldownRoutine != null)
            {
                Routines.Stop(cooldownRoutine);
                cooldownRoutine = null;
            }
            
            timer = duration;
        }

        protected override void OnUpdate(Packet packet)
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, duration);
            var charge = timer / duration;

            var board = packet.Get<Blackboard>();
            board.Set(timer / duration, "charge");
            
            Events.ZipCall(PlayerEvent.OnShake, shakeFactor * charge, maxShake);
            
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.Set(charge);
        }

        protected override void OnShutdown() => Routines.Start(CooldownRoutine());
        
        private IEnumerator CooldownRoutine()
        {
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            
            while (timer > 0.0f)
            {
                timer -= Time.deltaTime;
                HUD.Set(timer / duration);
                
                yield return new WaitForEndOfFrame();
            }

            timer = 0.0f;
            HUD.Set(0.0f);

            cooldownRoutine = null;
        }
    }
}