using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class ClickInput : RootNode
    {
        private bool previousState;
        
        protected override void OnBootup(Packet packet) => output = 0b_0010;
        
        public override IEnumerable<INode> Update(Packet packet)
        {
            var state = packet.Get<bool>();

            if (!state)
            {
                if (previousState)
                {
                    Command(packet, new ChannelRemovalCommand(0b_0001));
                    AddOutputChannel(packet, 0b_0010);
                    
                    Start(packet);
                }
                previousState = false;
                
                if (IsDone) return null;
                
                foreach (var branch in Branches) branch.Update(packet);
                OnUpdate(packet);
                
                if (IsDone) Close(packet);
            }
            else
            {
                if (!previousState)
                {
                    ChangeOutputMask(packet, output = 0b_0001);
                    Start(packet);
                }
                previousState = true;
                
                base.Update(packet);
            }
            
            return null;
        }
    }
}