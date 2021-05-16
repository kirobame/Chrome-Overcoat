using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class CinemachineShakeControl : BaseShakeControl, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            camera = new AnyValue<CinemachineVirtualCamera>();
            injections = new IValue[] { camera };
        }
        
        void IInjectionCallbackListener.OnInjectionDone(IRoot source) => noise = camera.Value.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        //--------------------------------------------------------------------------------------------------------------/

        [FoldoutGroup("Amplitude"), SerializeField] private AnimationCurve amplitudeMap;
        [FoldoutGroup("Amplitude"), SerializeField] private float maxAmplitude;
        
        [FoldoutGroup("Frequency"), SerializeField] private AnimationCurve frequencyMap;
        [FoldoutGroup("Frequency"), SerializeField] private float maxFrequency;

        private new IValue<CinemachineVirtualCamera> camera;
        private CinemachineBasicMultiChannelPerlin noise;
        
        protected override void Settle() { }
        protected override void Compute()
        {
            var ratio = Intensity / Range;
            noise.m_AmplitudeGain = amplitudeMap.Evaluate(ratio) * maxAmplitude;
            noise.m_FrequencyGain = frequencyMap.Evaluate(ratio) * maxFrequency;
        }
    }
}