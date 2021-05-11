using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetTimedNode : RootNode
    {
        public RetTimedNode(float duration) => this.duration = duration;

        public override bool IsDone => Branches.All(branch => branch.IsDone) && timer <= 0.0f;

        private float duration;
        private float timer;

        protected override void Open(Packet packet) => timer = duration;

        public override IEnumerable<INode> Use(Packet packet)
        {
            if (Branches.All(branch => branch.IsDone)) Prepare(packet);

            timer -= Time.deltaTime;
            foreach (var branch in Branches) branch.Update(packet);
            OnUse(packet);

            if (IsDone)
            {
                Close(packet);
            }

            return null;
        }
    }
}