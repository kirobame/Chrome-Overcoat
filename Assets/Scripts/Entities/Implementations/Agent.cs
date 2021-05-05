using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class Agent : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree weapon;
        
        [FoldoutGroup("References"), SerializeField] private NavMeshAgent navMesh;
        [FoldoutGroup("References"), SerializeField] private LineOfSight lineOfSight;
        [FoldoutGroup("References"), SerializeField] private Transform aim;
        [FoldoutGroup("References"), SerializeField] private Transform fireAnchor;

        private RemoteTaskTree runtimeWeapon;
        private ITaskTree taskTree;

        //--------------------------------------------------------------------------------------------------------------/

        void Start()
        {
            runtimeWeapon = Instantiate(weapon);
            runtimeWeapon.Bootup();
            
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("view", aim);
            board.Set("view.fireAnchor", fireAnchor);

            identity.Packet.Set(navMesh);
            identity.Packet.Set(lineOfSight);

            var playerColReference = "player.collider".Reference<Collider>(true);
            var fireAnchorReference = "view.fireAnchor".Reference<Transform>();
            var aimReference = "view".Reference<Transform>();

            taskTree = new RootNode();
            var conditionalNode = new CanSee(playerColReference, new PackettedValue<LineOfSight>());

            taskTree.Append(
                conditionalNode.Append(
                    new StopMoving(new PackettedValue<NavMeshAgent>()).Mask(0b_0001).Append(
                        new RootNode().Append(
                            new ComputeDirectionTo("shootDir", fireAnchorReference, playerColReference),
                            new LookAt(playerColReference, aimReference)), 
                        new PressNode(1.0f).Append(runtimeWeapon)),
                    new MoveTo(new PackettedValue<NavMeshAgent>(), "player".Reference<Transform>(true), aimReference).Mask(0b_0010).Append(
                        new Delay(0.5f))));
            
            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }
        
        void Update() => taskTree.Update(identity.Packet);
    }
}