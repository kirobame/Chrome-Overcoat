using UnityEngine;

namespace Chrome
{
    public abstract class Lerp : ProxyNode
    {
        public Lerp(float time) => this.time = time;
        
        private float time;
        private float timer;

        protected override void Open(Packet packet) => timer = 0.0f;

        protected override void OnUpdate(Packet packet)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                timer = time;
                Execute(packet, timer / time);

                isDone = true;
            }
            else Execute(packet, timer / time);
        }

        protected abstract void Execute(Packet packet, float ratio);
    }
}