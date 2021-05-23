using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class AttackTreeBuilder : TreeBuilder
    {
        [SerializeField] private Weapon weapon;
        
        public override ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            var playerColRef = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);
            
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var lineOfSightRef = AgentRefs.LINE_OF_SIGHT.Reference<LineOfSight>();
       
            return new RootNode().Append
                (
                    new CanSee(playerColRef, lineOfSightRef).Append
                    (
                        TT.IF_TRUE(new StopMoving(navRef)).Append
                        (
                            TT.WITH_PRIO(0, new RootNode()).Append
                            (
                                new ComputeDirectionTo("shootDir", fireAnchorRef, playerColRef),
                                new LookAt(playerColRef, pivotRef)
                            ),
                            TT.WITH_PRIO(1, new PressNode(1.0f)).Append
                            (
                                new WeaponNode(weapon)
                            )
                        ),
                        TT.IF_FALSE(new MoveTo(navRef, playerRef, pivotRef)).Append
                        (
                            new Delay(0.5f)
                        )
                    )
                );
        }
    }
}