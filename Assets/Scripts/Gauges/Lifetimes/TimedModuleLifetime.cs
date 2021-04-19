using UnityEngine;

namespace Chrome
{
    public struct TimedModuleLifetime : IGaugeModuleLifetime
    {
        public TimedModuleLifetime(float timer) => this.timer = timer;
        
        private float timer;
        
        public bool Update(Gauge source)
        {
            timer -= Time.deltaTime;
            return timer <= 0;
        }
    }
}