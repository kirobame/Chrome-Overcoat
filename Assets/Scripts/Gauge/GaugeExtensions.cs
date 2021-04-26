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

        public static void DIE(this Gauge gauge, Vector2 range, Lifetime lifetime)
        {
            var module = new GaugeInRangeModule(range, (value, percentage, state) =>
            {
                if (state == GaugeInRangeModule.State.EnteredRange) lifetime.End();
            });

            module.lifetime = new ConstantModuleLifetime();
            gauge.AddModule(module);
        }

        public static void PASSIVE(this Gauge gauge, float constant, Gauge other, float otherFactor)
        {
            var module = new GaugeInRangeModule(new Vector2(0.0f, 1.0f), (value, percentage, state) =>
            {
                if (state == GaugeInRangeModule.State.InRange) gauge.ADD((constant + other.Value / other.Max * otherFactor) * Time.deltaTime);
            });

            module.lifetime = new ConstantModuleLifetime();
            gauge.AddModule(module);
        }
    }
}