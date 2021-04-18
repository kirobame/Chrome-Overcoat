using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    // TO CORRECT !
    public class ClickInput : RootNode
    {
        private bool previousState;
        
        protected override void OnStart(Packet packet) => output = 0b_0001;
        
        public override IEnumerable<Node> Update(Packet packet)
        {
            var state = packet.Get<bool>();
            if (!state)
            {
                if (IsDone) return null;
                
                if (previousState)
                {
                    Shutdown();
                    previousState = false;
                }

                output = 0b_0010;
                
                Debug.Log("UPDATING OTHER PATH");
                OnUpdate(packet);
                UpdateCachedNodes(packet);
                
                if (CanBreak()) Shutdown();
            }
            else
            {
                previousState = true;
                return base.Update(packet);
            }

            return null;
        }
    }
}