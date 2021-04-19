using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Delay : ProxyNode
    {
        public Delay(float time) => this.time = time;

        [SerializeField] private float time;

        private float timer;

        protected override void OnStart(Packet packet)
        {
            IsLocked = true;
            timer = time;
        }

        protected override void OnUpdate(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                IsLocked = false;
                IsDone = true;
            }
        }
    }
}