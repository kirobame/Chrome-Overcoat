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
        [SerializeField] private GameObject shield;

        private ITaskTree taskTree;

        void Start()
        {
            var board = identity.Packet.Get<IBlackboard>();
            board.Set("collider", col);
            board.Set("transform", tr);
            board.Set("aim", aim);
            board.Set("shield", shield);

            identity.Packet.Set(navMesh);

            var ColReference = "collider".Reference<Collider>();
            var trReference = "transform".Reference<Transform>();
            var aimReference = "aim".Reference<Transform>();
            var shieldReference = "shield".Reference<GameObject>();

            var player = "player".Reference<Transform>(true);
            var playerColReference = "player.collider".Reference<Collider>(true);
            var TargetLineOfSight = "player.lineOfSight".Reference<LineOfSight>(true);

            taskTree = new RootNode();

            var lookAt = new RootNode().Append(new LookAt(playerColReference, aimReference));

            taskTree.Append(
                new IsSeen(ColReference, TargetLineOfSight, aimReference).Append(
                    //Seen
                    new StopMoving().Mask(0b_0001).Append(
                        new ShieldUp(shieldReference),
                        lookAt
                        ),
                    //Not seen
                    new IsCloseTo(trReference, player, 3f).Mask(0b_0010).Append(
                        //At range
                        lookAt,
                        new AttackCac(identity, aimReference, 5).Mask(0b_0001).Append(
                            new Delay(0.5f),
                            new ShieldDown(shieldReference)),
                        //Not at range
                        new MoveTo(new PackettedValue<NavMeshAgent>(), player).Mask(0b_0010).Append(
                            new Delay(0.5f),
                            new ShieldDown(shieldReference))
                        //Both
                        //new ShieldDown().Mask(0b_0011)
                        )
                    )
                );

            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }

        void Update() => taskTree.Update(identity.Packet);
    }
}
