using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class JeffSetReady : TaskNode
    {
        public JeffSetReady(bool ready)
        {
            this.ready = ready;
        }

        bool ready;

        protected override void OnUse(Packet packet)
        {
            var rootRef = Refs.ROOT.Reference<Transform>();

            if (Blackboard.Global.TryGet<Dictionary<Transform, bool>>("AI.Jeffs", out var jeffs) && rootRef.IsValid(packet))
            {
                jeffs[rootRef.Value] = ready;
                Blackboard.Global.Set<Dictionary<Transform, bool>>("AI.Jeffs", jeffs);
            }

            isDone = true;
        }
    }
}
