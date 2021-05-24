using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class KyleSeekTreeBuilder : TreeBuilder
    {
        public override ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            
            return TT.START(GoalDefinition.Seek).Append
            (
                new MoveTo(navRef, playerRef, pivotRef).Append
                (
                    new Delay(0.5f)
                )
            );
        }
    }
}