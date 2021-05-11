using System;
using System.Collections.Generic;
using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class StandardAgentImplementation : MonoBehaviour, ILifebound, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
//
        public event Action<ILifebound> onDestruction;

        public bool IsActive => true;
        
        [FoldoutGroup("Values"), SerializeField] private Weapon weapon;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;

        private ITaskTree taskTree;
        private bool hasBeenBootedUp;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            hasBeenBootedUp = false;
            
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
        }
        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup()
        {
            hasBeenBootedUp = true;
            
            taskTree.Bootup(packet);
            taskTree.Prepare(packet);
        }
        public void Shutdown()
        {
            hasBeenBootedUp = false;
            taskTree.Shutdown(packet);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            var navReference = AgentRefs.NAV.Reference<NavMeshAgent>();
            var playerColReference = $"{PlayerRefs.BOARD}.{Refs.COLLIDER}".Reference<Collider>(ReferenceType.SubGlobal);
            var pivotReference = Refs.PIVOT.Reference<Transform>();
            var fireAnchorReference = Refs.FIREANCHOR.Reference<Transform>(); ;
            
            taskTree = new RootNode();
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
        }
        
        void Update()
        {
            if (!hasBeenBootedUp) return;
            taskTree.Use(packet);
        }
    }
}