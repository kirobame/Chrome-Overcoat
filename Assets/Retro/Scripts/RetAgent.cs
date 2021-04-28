using System.Collections;
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

        private const string REF_SHOOTDIR = "shootDirection";
        
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        [FoldoutGroup("Dependencies"), SerializeField] private Transform head;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform pivot;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform fireAnchor;
        [FoldoutGroup("Dependencies"), SerializeField] private NavMeshAgent navMesh;

        [FoldoutGroup("Values"), SerializeField] private RetGun gun;
        [FoldoutGroup("Values"), SerializeField] private float reset;
        [FoldoutGroup("Values"), SerializeField] private float distance;

        private ITaskTree taskTree;

        public void Bootup()
        {
            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }
        public void Shutdown() => taskTree.Shutdown(identity.Packet);
        
        void Start()
        {
            identity.Packet.Set(navMesh);

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
            
            var isTargetVisible = new IsVisible(REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), LayerMask.GetMask("Environment"));
            var isInProximityOfTarget = new IsInProximityOf(REF_ROOT.Reference<Transform>(), REF_TARGET.Reference<Transform>(), distance);
            var conditionalNode = new CompositeConditionalNode(isTargetVisible.Chain(ConditionalOperator.AND), isInProximityOfTarget);
            
            taskTree = new RootNode().Append(
                conditionalNode.Append(
                    new StopMoving(new PackettedValue<NavMeshAgent>()).Mask(0b_0001).Append(
                        new RootNode().Append(
                            new LookAt(REF_TARGETCOL.Reference<Collider>(), REF_PIVOT.Reference<Transform>()),
                            new ComputeDirectionTo(REF_SHOOTDIR, REF_HEAD.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>())),
                        new RetGunNode(REF_SHOOTDIR.Reference<Vector3>(), REF_FIREANCHOR.Reference<Transform>(), REF_TARGETCOL.Reference<Collider>(), gun)),
                    new MoveTo(new PackettedValue<NavMeshAgent>(), REF_TARGET.Reference<Transform>()).Mask(0b_0010).Append(
                        new Delay(reset))));
            
            Bootup();
        }
        
        void Update() => taskTree.Update(identity.Packet);
    }
}