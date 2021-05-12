using System;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class AttackTreeBuilder : ITreeBuilder
    {
        [SerializeField] private Weapon weapon;
        
        public ITaskTree Build()
        {
            var navReference = AgentRefs.NAV.Reference<NavMeshAgent>();
            var playerColReference = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);
            var pivotReference = Refs.PIVOT.Reference<Transform>();
            var fireAnchorReference = Refs.FIREANCHOR.Reference<Transform>(); ;
            
            var taskTree = new RootNode();
            var conditionalNode = new CanSee(playerColReference, AgentRefs.LINE_OF_SIGHT.Reference<LineOfSight>());
            
            taskTree.Append(
                conditionalNode.Append(
                    new StopMoving(navReference).Mask(0b_0001).Append(
                        new RootNode().Append(
                            new ComputeDirectionTo("shootDir", fireAnchorReference, playerColReference),
                            new LookAt(playerColReference, pivotReference)), 
                        new PressNode(1.0f).Append(
                            new WeaponNode(weapon))),
                    new MoveTo(navReference, $"{PlayerRefs.BOARD}.{Refs.ROOT}".Reference<Transform>(ReferenceType.SubGlobal), pivotReference).Mask(0b_0010).Append(
                        new Delay(0.5f))));

            return taskTree;
        }
    }
}