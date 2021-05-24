using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class KyleFleeTreeBuilder : TreeBuilder
    {
        [SerializeField] private float cooldown;
        [SerializeField] private float speed;
        [SerializeField] private float range;
        [SerializeField] private float height;
        [SerializeField] private int resolution;

        public override ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var colRef = Refs.COLLIDER.Reference<Collider>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var viewRef = Refs.VIEW.Reference<Transform>();

            return TT.START(GoalDefinition.Flee).Append
            (
                new SetLocalReference<float>(KyleRefs.FLEE_COOLDOWN, cooldown),
                new ResetAim(pivotRef).Append
                (
                    TT.BLOCK(new LeapOut(speed, range, height, resolution, navRef, colRef, playerRef, viewRef)).Append
                    (
                        new Delay(1.5f) // Should directly let go of goal ?
                    )
                )
            );
        }
    }
}