using System;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    public class BurstCountNode : TaskNode
    {        
        
        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void OnUse(Packet packet)
        {
            if (packet.TryGet<IBlackboard>(out var bb))
            {
                var ammount = bb.Get<int>("weapon.burst");
                ammount--;
                bb.Set<int>("weapon.burst", ammount);
            }

            isDone = true;
        }
    }
}