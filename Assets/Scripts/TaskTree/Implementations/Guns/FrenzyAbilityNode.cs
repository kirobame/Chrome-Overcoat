using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class FrenzyAbilityNode : RootNode
    {
        private bool previousState;
        
        protected override void OnBootup(Packet packet)
        {
            previousState = false;
            output = 0b_0010;
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            var state = packet.Get<bool>();
            var board = packet.Get<IBlackboard>();

            if (!state)
            {
                if (previousState)
                {
                    ChangeOutputMask(packet, 0b_0010);
                    Prepare(packet);
                }
                previousState = false;
                
                if (IsDone) return null;
                
                foreach (var branch in Branches) branch.Update(packet);
                OnUse(packet);
                
                if (IsDone) Close(packet);
            }
            else
            {
                if (!previousState)
                {
                    ChangeOutputMask(packet, output = 0b_0001);
                    Prepare(packet);
                }
                previousState = true;
                
                base.Use(packet);

                packet.Set(false);
                board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).business--;
            }
            
            return null;
        }
    }
}