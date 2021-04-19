using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Chrome
{
    public class Gauge : MonoBehaviour
    {
        public float Max => max;
        public float Value => goal;

        [SerializeField] private float max;
        [SerializeField] private Slider slider;
        [SerializeField] private Gauges address;
        
        [Space, SerializeField] private float smoothing;
        [SerializeField] private AnimationCurve map;

        private float goal;
        private float value;
        private float mappedValue;
        
        private float velocity;
        
        private List<GaugeModule> modules = new List<GaugeModule>();

        //--------------------------------------------------------------------------------------------------------------/
        
        void Awake()
        {
            goal = Mathf.Clamp01(slider.value) * max;
            value = goal;
            
            slider.maxValue = max;
            
            Repository.Register(address, this);
        }
        void OnDestroy() => Repository.Unregister(address);
        
        void Update()
        {
            for (var i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                module.Update();
            }

            var affects = modules.Where(module => module is IGaugeAffect).Cast<IGaugeAffect>();
            var impacts = modules.Where(module => module is IGaugeImpact).Cast<IGaugeImpact>();

            foreach (var affect in affects) { foreach (var impact in impacts) affect.Affect(impact); }
            foreach (var impact in impacts) goal = impact.ComputeImpact(goal);

            goal = Mathf.Clamp(goal, 0, max);
            modules.RemoveAll(module => module.IsDone);

            mappedValue = map.Evaluate(goal / max) * max;
            value = Mathf.SmoothDamp(value, mappedValue, ref velocity, smoothing);
            slider.value = value;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public void ResetAt(float value, bool inPercent)
        {
            if (inPercent) value = Mathf.Clamp01(value) * max;
            
            modules.Clear();
            goal = value;
        }
        
        public void AddModule(GaugeModule module)
        {
            module.Initialize(this);
            modules.Add(module);
        }
    }
}