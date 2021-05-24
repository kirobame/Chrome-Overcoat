using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class JeffSynchCheck : Condition
    {
        private IValue<Transform> playerTr;

        public override bool Check(Packet packet)
        {
            var root = Refs.ROOT.Reference<Transform>();
            /*
            if (Blackboard.Global.TryGet<Dictionary<Collider, bool>>("AI.Jeffs", out var jeffs) && colRef.IsValid(packet))
            {
                jeffs[colRef.Value] = true;
                Blackboard.Global.Set<Dictionary<Collider, bool>>("AI.Jeffs", jeffs);
            }*/

            return JeffManager.IsSynchAssaultReady();
        }
        
        /*protected override void OnPrepare(Packet packet)
        {
            
            playerTr = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            playerTr.IsValid(packet);
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            pivotRef.IsValid(packet);

            var pos = Quaternion.Euler(0, 30, 0) * pivotRef.Value.position;
            

            isDone = false;
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            if (JeffManager.IsSynchAssaultReady(packet))
                isDone = true;
            return base.Use(packet);
        }*/
    }
}
