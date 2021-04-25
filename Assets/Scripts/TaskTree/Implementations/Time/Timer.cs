using UnityEngine;

namespace Chrome
{
    public class Timer : ConditionalNode
    {
        public Timer(IValue<float> time) => this.time = time;
        
        private IValue<float> time;
        private float timer;

        protected override void OnBootup(Packet packet)
        {
            if (time.IsValid(packet)) timer = time.Value;
            else timer = 0.0f;
        }

        protected override bool Check(Packet packet)
        {
            timer -= Time.deltaTime;
            return timer < 0.0f;
        }
    }
}