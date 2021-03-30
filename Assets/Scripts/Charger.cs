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
        [SerializeField] private AnimationCurve shakeAmplitude;
        [SerializeField] private float shakeAmplitudeGoal;
        [SerializeField] private AnimationCurve shakeFrequency;
        [SerializeField] private float shakeFrequencyGoal;
        
        private float progress;
        private ChargeHUD HUD;

        public override void Bootup(GunControl control)
        {
            HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.IndicateThreshold(threshold / time);
            
            base.Bootup(control);
        }

        protected override EventArgs OnStart(Aim aim, EventArgs args)
        {
            MoveControl.canSprint = false;
            return args;
        }

        protected override EventArgs OnUpdate(Aim aim, EventArgs args)
        {
            progress = Mathf.Clamp(aim.pressTime, 0.0f, time);
            HUD.Set(progress / time);

            var camera = Repository.Get<CinemachineVirtualCamera>(Reference.VirtualCamera);
            var noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            var ratio = Mathf.InverseLerp(threshold, time, progress);
            noise.m_AmplitudeGain = Mathf.Lerp(0.0f, shakeAmplitudeGoal, shakeAmplitude.Evaluate(ratio));
            noise.m_FrequencyGain = Mathf.Lerp(0.0f, shakeFrequencyGoal, shakeFrequency.Evaluate(ratio));
            
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

            MoveControl.canSprint = true;
            base.End(aim, args);
        }
        protected override EventArgs OnEnd(Aim aim, EventArgs args)
        {
            Routines.Start(CooldownRoutine());
            
            var camera = Repository.Get<CinemachineVirtualCamera>(Reference.VirtualCamera);
            var noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            var startingAmplitude = noise.m_AmplitudeGain;
            var startingFrequency = noise.m_FrequencyGain;

            Routines.Start(Routines.RepeatFor(0.1f, ratio =>
            {
                noise.m_AmplitudeGain = Mathf.Lerp(startingAmplitude, 0.0f, ratio);
                noise.m_FrequencyGain = Mathf.Lerp(startingFrequency, 0.0f, ratio);

            }, new YieldFrame()));
            
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