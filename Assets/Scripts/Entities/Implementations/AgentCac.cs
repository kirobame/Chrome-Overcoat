using Flux;
using Flux.Data;
using Sirenix.OdinInspector;
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

        [FoldoutGroup("References"), SerializeField] private Transform tr;
        [FoldoutGroup("References"), SerializeField] private Collider col;
        [FoldoutGroup("References"), SerializeField] private NavMeshAgent navMesh;
        [FoldoutGroup("References"), SerializeField] private Transform aim;
        [FoldoutGroup("References"), SerializeField] private Shield shield;

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
            var shieldReference = "shield".Reference<Shield>();

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
                        //new ShieldDown(shieldReference).Mask(0b_0011)
                        )
                    )
                );

            taskTree.Bootup(identity.Packet);
            taskTree.Start(identity.Packet);
        }

        void Update() => taskTree.Update(identity.Packet);
    }
}
