using UnityEngine;

namespace Chrome
{
    public static class GaugeExtensions
    {
        public static void ADD(this Gauge gauge, float value, float time = 0.0f)
        {
            var module = new GaugeAdditionModule(value);
            if (time > 0.0f) module.lifetime = new TimedModuleLifetime(Mathf.Abs(time));
            
            gauge.AddModule(module);
        }
        
        public static void MUL(this Gauge gauge, float value, float time = 0.0f)
        {
            var module = new GaugeMultiplicationModule(value);
            if (time > 0.0f) module.lifetime = new TimedModuleLifetime(time);
            
            gauge.AddModule(module);
        }
        
        public static void DIV(this Gauge gauge, float value, float time = 0.0f)
        {
            var module = new GaugeDivisionModule(value);
            if (time > 0.0f) module.lifetime = new TimedModuleLifetime(time);
            
            gauge.AddModule(module);
        }
    }
}