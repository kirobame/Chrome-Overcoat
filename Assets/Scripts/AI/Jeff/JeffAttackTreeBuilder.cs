using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class JeffAttackTreeBuilder : TreeBuilder
    {
        
        public override ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            var playerColRef = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);
            
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var rootRef = Refs.ROOT.Reference<Transform>();
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var fireDirRef = Refs.SHOOT_DIRECTION.Reference<Vector3>();
            var weaponRef = AgentRefs.WEAPON.Reference<Weapon>();

            RootNode AttackRootNode;


            return AttackRootNode = new RootNode().Append
            (
                TT.IF(new IsCloseTo(15f, pivotRef, playerRef)).Append
                (
                    TT.IF_TRUE(new StopMoving(navRef)).Append
                    (
                            TT.IF(new CanSee(pivotRef, playerColRef)).Append
                            (
                                TT.IF_TRUE(new StopMoving(navRef)).Append
                                (
                                    TT.WITH_PRIO(0, new RootNode()).Append
                                    (
                                        new ComputeDirectionTo(fireDirRef, fireAnchorRef, playerColRef),
                                        new LookAt(playerColRef, pivotRef),

                                        new StrafAround(navRef, pivotRef, rootRef, playerRef)
                                    ),
                                    TT.WITH_PRIO(1, new JeffPressNode(2.0f)).Append
                                    (
                                        new WeaponNode(weaponRef)
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
                        new Delay(0.5f)
                    )
                )
            );
        }
    }
}