using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome.Retro
{
    public class RetAgent : MonoBehaviour, ILifebound, ILink<IIdentity>
    {
        public const string REF_ROOT = "self";
        public const string REF_HEAD = "self.head";
        public const string REF_PIVOT = "self.pivot";
        public const string REF_FIREANCHOR = "self.pivot.fireAnchor";
        public const string REF_TARGET = "target";
        public const string REF_TARGETCOL = "target.collider";

        protected const string REF_SHOOTDIR = "shootDirection";
        
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        protected IIdentity identity;

        [FoldoutGroup("Dependencies"), SerializeField] protected RetAgentLookAt lookAt;
        [FoldoutGroup("Dependencies"), SerializeField] protected Transform head;
        [FoldoutGroup("Dependencies"), SerializeField] protected Transform pivot;
        [FoldoutGroup("Dependencies"), SerializeField] protected Transform fireAnchor;
        [FoldoutGroup("Dependencies"), SerializeField] protected NavMeshAgent navMesh;

        [FoldoutGroup("Values"), SerializeField] protected RetGun gun;
        [FoldoutGroup("Values"), SerializeField] protected float reset;
        [FoldoutGroup("Values"), SerializeField] protected float distance;

        protected ITaskTree taskTree;

        public void Bootup()
        {
            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }
        public void Shutdown() => taskTree.Shutdown(identity.Packet);
        
        void Start()
        {
            identity.Packet.Set(navMesh);
            identity.Packet.Set(lookAt);

            var board = identity.Packet.Get<IBlackboard>();
            board.Set(REF_ROOT, identity.Root);
            board.Set(REF_HEAD, head);
            board.Set(REF_PIVOT, pivot);
            board.Set(REF_FIREANCHOR, fireAnchor);
            
            var playerBoard = Blackboard.Global.Get<IBlackboard>(RetPlayerBoard.REF_SELF);
            var targetTransform = playerBoard.Get<IIdentity>(RetPlayerBoard.REF_IDENTITY).Root;
            var targetCollider = playerBoard.Get<Collider>(RetPlayerBoard.REF_COLLIDER);
            
            board.Set(REF_TARGET, targetTransform);
            board.Set(REF_TARGETCOL, targetCollider);
            
            BuildTree();
            Bootup();
        }
        
        void Update() => taskTree.Update(identity.Packet);

        protected virtual void BuildTree()
        {
            var isTargetVisible = new IsVisible(REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), LayerMask.GetMask("Environment"));
            var isInProximityOfTarget = new IsInProximityOf(REF_ROOT.Reference<Transform>(), REF_TARGET.Reference<Transform>(), distance);
            var conditionalNode = new CompositeConditionalNode(isTargetVisible.Chain(ConditionalOperator.AND), isInProximityOfTarget);
            
            taskTree = new RootNode().Append(
                conditionalNode.Append(
                    new StopMoving(new PackettedValue<NavMeshAgent>()).Mask(0b_0001).Append(
                        new RootNode().Append(
                            new RetLookAt(REF_TARGETCOL.Reference<Collider>(), new PackettedValue<RetAgentLookAt>()),
                            new ComputeDirectionTo(REF_SHOOTDIR, REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), Vector3.up * 0.225f)),
                        new RetGunNode(REF_SHOOTDIR.Reference<Vector3>(), REF_FIREANCHOR.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), gun)),
                    new RetMoveTo(new PackettedValue<NavMeshAgent>(), REF_TARGET.Reference<Transform>(), new PackettedValue<RetAgentLookAt>()).Mask(0b_0010).Append(
                        new Delay(reset))));
        }
    }
}