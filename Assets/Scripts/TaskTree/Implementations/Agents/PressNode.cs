using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class PressNode : RootNode
    {
        public PressNode(float duration) => this.duration = duration;

        public override bool IsDone => isDone;

        [SerializeField] private float duration;

        private bool isDone;
        private float timer;

        protected override void OnPrepare(Packet packet)
        {
            isDone = false;
            timer = duration;
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                packet.Set(false);
                base.Use(packet);

                isDone = true;
                return null;
            }
            else
            {
                packet.Set(true);
                return base.Use(packet);
            }
        }
    }
}