using UnityEngine;

namespace Chrome
{
    public abstract class LerpNode : TaskedNode
    {
        public LerpNode(float time) => this.time = time;
        
        private float time;
        private float timer;

        protected override void Open(Packet packet) => timer = 0.0f;

        protected override void OnUse(Packet packet)
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