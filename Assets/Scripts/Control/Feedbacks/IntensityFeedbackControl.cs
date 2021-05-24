using System;
using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class IntensityFeedbackControl : MonoBehaviour, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            intensity = new AnyValue<IntensityControl>();
            injections = new IValue[] { intensity };
        }

        //--------------------------------------------------------------------------------------------------------------/

        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;
        
        [SerializeField] private float settleTime;
        [SerializeField] private AnimationCurve lowMap;
        [SerializeField] private AnimationCurve highMap;
        
        private IValue<IntensityControl> intensity;
        private UnityEngine.Rendering.Volume low;
        private UnityEngine.Rendering.Volume high;

        private bool state;

        //--------------------------------------------------------------------------------------------------------------/

        void Start()
        {
            state = false;
            
            low = Repository.Get<UnityEngine.Rendering.Volume>(Volume.IntensityLow);
            high = Repository.Get<UnityEngine.Rendering.Volume>(Volume.IntensityHigh);
        }
        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup(byte code) => state = true;
        public void Shutdown(byte code)
        {
            state = false;
            
            StartCoroutine(SettleRoutine(low));
            StartCoroutine(SettleRoutine(high));
        }

        private IEnumerator SettleRoutine(UnityEngine.Rendering.Volume volume)
        {
            var time = Mathf.InverseLerp(0.0f, settleTime, volume.weight);
            while (time < settleTime)
            {
                time += Time.deltaTime;
                volume.weight = 1.0f - time / settleTime;
                
                yield return new WaitForEndOfFrame();
            }

            volume.weight = 0.0f;
        }
        
        void Update()
        {
            if (!state) return;
            var ratio = intensity.Value.Ratio;

            if (ratio < 0.5f)
            {
                low.weight = lowMap.Evaluate((0.5f - ratio) * 2.0f);
                high.weight = 0.0f;
            }
            else
            {
                low.weight = 0.0f;
                high.weight = highMap.Evaluate((ratio - 0.5f) * 2.0f);
            }
        }
    }
}