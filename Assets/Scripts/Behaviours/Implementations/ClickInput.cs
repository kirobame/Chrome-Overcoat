using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class ClickInput : RootNode
    {
        protected override void Open(Packet packet) => output = 0b_0010;

        public override IEnumerable<INode> Update(Packet packet)
        {
            var state = packet.Get<bool>();

            if (!state)
            {
                if (output != 0b_0010)
                {
                    ChangeOutput(packet, 0b_0010);
                    Start(packet);
                }

                if (IsDone) return null;
                
                OnUpdate(packet);
                UpdateCachedNodes(packet);
                
                if (CanBreak()) Close(packet);
            }
            else
            {
                if (output != 0b_0001)
                {
                    ChangeOutput(packet, output = 0b_0001);
                    Start(packet);
                }
                
                base.Update(packet);
            }

            return null;
        }
    }
}