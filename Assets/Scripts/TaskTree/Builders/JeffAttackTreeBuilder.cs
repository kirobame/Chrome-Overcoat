using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class JeffAttackTreeBuilder : ITreeBuilder
    {
        [SerializeField] private Weapon weapon;
        
        public ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            var playerColRef = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);
            
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var rootRef = Refs.ROOT.Reference<Transform>();
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var lineOfSightRef = AgentRefs.LINE_OF_SIGHT.Reference<LineOfSight>();

            RootNode AttackRootNode;


            return AttackRootNode = new RootNode().Append
            (
                new IsCloseTo(pivotRef, playerRef, 15f).Append
                (
                    TT.IF_TRUE(new StopMoving(navRef)).Append
                    (
                        //new JeffSetReady(true),
                            new CanSee(playerColRef, lineOfSightRef).Append
                            (
                                TT.IF_TRUE(new StopMoving(navRef)).Append
                                (
                                    TT.WITH_PRIO(0, new RootNode()).Append
                                    (
                                        new ComputeDirectionTo("shootDir", fireAnchorRef, playerColRef),
                                        new LookAt(playerColRef, pivotRef),

                                        new StrafAround(navRef, pivotRef, rootRef, playerRef)
                                    ),
                                    TT.WITH_PRIO(1, new JeffPressNode(2.0f)).Append
                                    (
                                        new WeaponNode(weapon)
                                    )
                                ),
                                TT.IF_FALSE(new MoveTo(navRef, playerRef, pivotRef)).Append
                                (
                                    new Delay(0.5f)
                                )
                            )
                    ),
                    TT.IF_FALSE(new MoveTo(navRef, playerRef, pivotRef)).Append
                    (
                        //new JeffSetReady(false),
                        new Delay(0.5f)
                    )
                )
            );
        }
    }
}