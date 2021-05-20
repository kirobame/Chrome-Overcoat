using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class KyleAttackTreeBuilder : ITreeBuilder
    {
        public ITaskTree Build()
        {
            var playerRef = $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal);
            var playerColRef = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);

            var weaponRef = AgentRefs.WEAPON.Reference<Weapon>();
            var shootDirectionRef = Refs.SHOOT_DIRECTION.Reference<Vector3>();
            var navRef = AgentRefs.NAV.Reference<NavMeshAgent>();
            var pivotRef = Refs.PIVOT.Reference<Transform>();
            var fireAnchorRef = Refs.FIREANCHOR.Reference<Transform>();
            var viewRef = Refs.VIEW.Reference<Transform>();

            return TT.START(GoalDefinition.Attack).Append
            (
                TT.IF(new CanSee(viewRef, playerColRef, shootDirectionRef)).Append
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