using Flux;
using Flux.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class AgentCac : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        [SerializeField] private Transform tr;
        [SerializeField] private Collider col;

        [Space, SerializeField] private NavMeshAgent navMesh;
        [SerializeField] private Transform aim;

        private ITaskTree taskTree;

        void Start()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("collider", col);
            board.Set("transform", tr);
            board.Set("aim", aim);

            identity.Packet.Set(navMesh);

            var ColReference = "collider".Reference<Collider>();
            var aimReference = "aim".Reference<Transform>();
            var trReference = "transform".Reference<Transform>();

            var player = "player".Reference<Transform>(true);
            var playerColReference = "player.collider".Reference<Collider>(true);
            var TargetLineOfSight = "player.lineOfSight".Reference<LineOfSight>(true);

            taskTree = new RootNode();

            var IsSeen = new CanSee(ColReference, TargetLineOfSight);

            taskTree.Append(
                IsSeen.Append(
                    //Seen
                    new StopMoving().Mask(0b_0001).Append(
                        new ShieldUp(),
                        new RootNode().Append(
                            new LookAt(playerColReference, aimReference))),
                    //Not seen
                    new IsCloseTo(trReference, player, 3f).Mask(0b_0010).Append(
                        //At range
                        new AttackCac().Mask(0b_0001).Append(
                            new Delay(0.5f)),
                        new RootNode().Mask(0b_0001).Append(
                            new LookAt(playerColReference, aimReference)),
                        //Not at range
                        new MoveTo(new PackettedValue<NavMeshAgent>(), player).Mask(0b_0010).Append(
                            new Delay(0.5f)),
                        //Both
                        new ShieldDown().Mask(0b_0011)
                        )
                    )
                );

            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }

        void Update() => taskTree.Update(identity.Packet);
    }
}
