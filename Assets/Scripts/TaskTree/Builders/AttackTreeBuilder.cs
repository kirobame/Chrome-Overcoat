using System;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Chrome
{
    [Serializable]
    public class AttackTreeBuilder : TreeBuilder
    {
        public override ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            var playerColRef = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);
            
            var shootDirectionRef = Refs.SHOOT_DIRECTION.Reference<Vector3>();
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var viewRef = Refs.VIEW.Reference<Transform>();
            var weaponRef = AgentRefs.WEAPON.Reference<Weapon>();

            return new RootNode().Append
                (
                    TT.IF(new CanSee(viewRef, playerColRef)).Append
                    (
                        TT.IF_TRUE(new StopMoving(navRef)).Append
                        (
                            TT.WITH_PRIO(0, new RootNode()).Append
                            (
                                new ComputeDirectionTo(shootDirectionRef, fireAnchorRef, playerColRef),
                                new LookAt(playerColRef, pivotRef)
                            ),
                            TT.WITH_PRIO(1, new PressNode(1.0f)).Append
                            (
                                new WeaponNode(weaponRef)
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