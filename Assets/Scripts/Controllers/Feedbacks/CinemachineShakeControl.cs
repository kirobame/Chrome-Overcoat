using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class CinemachineShakeControl : BaseShakeControl
    {
        [FoldoutGroup("Effect"), SerializeField] private new CinemachineVirtualCamera camera;

        [FoldoutGroup("Amplitude"), SerializeField] private AnimationCurve amplitudeMap;
        [FoldoutGroup("Amplitude"), SerializeField] private float maxAmplitude;
        
        [FoldoutGroup("Frequency"), SerializeField] private AnimationCurve frequencyMap;
        [FoldoutGroup("Frequency"), SerializeField] private float maxFrequency;
        
        private CinemachineBasicMultiChannelPerlin noise;

        void Start() => noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        protected override void Settle() { }
        protected override void Compute()
        {
            var ratio = Intensity / Range;
            noise.m_AmplitudeGain = amplitudeMap.Evaluate(ratio) * maxAmplitude;
            noise.m_FrequencyGain = frequencyMap.Evaluate(ratio) * maxFrequency;
        }
    }
}