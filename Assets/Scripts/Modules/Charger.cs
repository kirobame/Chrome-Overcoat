using System;
using System.Collections;
using Cinemachine;
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
        [SerializeField] private float threshold;
        [SerializeField] private float shakeFactor;
        [SerializeField] private float maxShake;
        
        private float progress;
        private ChargeHUD HUD;

        public override void Bootup(GunControl control)
        {
            HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.IndicateThreshold(threshold / time);
            
            base.Bootup(control);
        }

        protected override EventArgs OnUpdate(Aim aim, EventArgs args)
        {
            progress = Mathf.Clamp(aim.pressTime, 0.0f, time);
            HUD.Set(progress / time);

            var ratio = Mathf.InverseLerp(threshold, time, progress);
            Events.ZipCall(PlayerEvent.OnShake, shakeFactor * ratio, maxShake);

            return args;
        }

        public override void End(Aim aim, EventArgs args)
        {
            if (progress < threshold)
            {
                progress = 0.0f;
                HUD.Set(0.0f);
                
                return;
            }

            base.End(aim, args);
        }
        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            Routines.Start(CooldownRoutine());
            return new WrapperArgs<float>(Mathf.InverseLerp(threshold, time, progress));
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

            progress = 0.0f;
            HUD.Set(0.0f);
            
            IsActive = true;
        }
    }
}