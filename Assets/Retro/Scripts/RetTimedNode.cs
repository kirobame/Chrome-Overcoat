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

        public override IEnumerable<INode> Update(Packet packet)
        {
            if (Branches.All(branch => branch.IsDone)) Start(packet);

            timer -= Time.deltaTime;
            foreach (var branch in Branches) branch.Update(packet);
            OnUpdate(packet);

            if (IsDone)
            {
                Debug.Log("DONE");
                Close(packet);
            }

            return null;
        }
    }
}