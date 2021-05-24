using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class JeffPressNode : RootNode
    {
        public JeffPressNode(float duration) => this.duration = duration;

        public override bool IsDone => isDone;

        [SerializeField] private float duration;

        private bool isDone;
        private float timer;

        protected override void OnPrepare(Packet packet)
        {
            isDone = false;
            timer = duration;

            IsLocked = false;

            packet.TryGet<IBlackboard>( out var bb);
            bb.Set<int>("weapon.burst", 6);
            //Debug.Log("PressNode Prepare" + bb.Get<int>("weapon.burst"));
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            packet.TryGet<IBlackboard>(out var bb);
            if (bb.Get<int>("weapon.burst") <= 0)
            {
                packet.Set(false);
                base.Use(packet);

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    IsLocked = true;
                    isDone = true;
                }
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