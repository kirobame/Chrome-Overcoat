using System;
using System.Collections;
using Flux;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Charger : GunPart
    {
        [SerializeField] private float time;
        
        private float progress;
        private float velocity;
        
        private ChargeHUD HUD;

        public override void Bootup(GunControl control)
        {
            HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            base.Bootup(control);
        }

        protected override EventArgs OnStart(Aim aim, EventArgs args)
        {
            velocity = 0.0f;
            return args;
        }

        protected override EventArgs OnUpdate(Aim aim, EventArgs args)
        {
            var target = Mathf.Clamp(aim.pressTime, 0.225f, time);
            progress = Mathf.SmoothDamp(progress, target, ref velocity, 0.1f);
            
            HUD.Set(progress / time);
            return args;
        }
        
        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            Routines.Start(CooldownRoutine());
            return new WrapperArgs<float>(progress / time);
        }

        private IEnumerator CooldownRoutine()
        {
            IsActive = false;
            
            while (progress > 0.0f)
            {
                progress -= Time.deltaTime;
                HUD.Set(progress / time);
                
                yield return new WaitForEndOfFrame();
            }
            
            HUD.Set(0.0f);
            IsActive = true;
        }
    }
}