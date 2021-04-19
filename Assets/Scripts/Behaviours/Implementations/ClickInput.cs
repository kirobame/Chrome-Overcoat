using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class ClickInput : RootNode
    {
        public ClickInput() => output = 0b_0010;
        
        public override IEnumerable<Node> Update(Packet packet)
        {
            var state = packet.Get<bool>();

            if (!state)
            {
                if (output != 0b_0010)
                {
                    output = 0b_0010;
                    Start(packet);
                }

                if (IsDone) return null;
                
                OnUpdate(packet);
                UpdateCachedNodes(packet);
                
                if (CanBreak()) Shutdown();
            }
            else
            {
                if (output != 0b_0001)
                {
                    output = 0b_0001;
                    Start(packet);
                }
                
                base.Update(packet);
            }

            return null;
        }
    }
}