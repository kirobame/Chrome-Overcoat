using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Delay : TaskNode
    {
        public Delay(float time) => this.time = time;

        [SerializeField] private float time;

        private float timer;

        protected override void OnPrepare(Packet packet)
        {
            IsLocked = true;
            timer = time;
        }

        protected override void OnUse(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                IsLocked = false;
                isDone = true;
            }
        }
    }
}