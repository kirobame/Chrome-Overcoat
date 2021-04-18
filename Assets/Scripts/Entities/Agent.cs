using Flux;
using Flux.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class Agent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMesh;
        [SerializeField] private LineOfSight lineOfSight;
        
        [Space, SerializeField] private Transform aim;
        [SerializeField] private Transform fireAnchor;

        [Space, SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        
        private RootNode behaviourTree;
        private Blackboard board;
        private Packet packet;

        void Awake()
        {
            board = new Blackboard();
            board.Set(aim, "aim");
            board.Set(fireAnchor, "aim.fireAnchor");
            
            packet = new Packet();
            packet.Set(navMesh);
            packet.Set(lineOfSight);
            packet.Set(board);

            var playerBodyReference = "player.body".Reference<PhysicBody>(true);
            var aimReference = "aim".Reference<Transform>();
            
            behaviourTree = new RootNode();
            var conditionalNode = new CanSee(playerBodyReference, new PackettedValue<LineOfSight>());
            
            behaviourTree.Append(
                conditionalNode.Append(
                    new StopMoving().Mask(0b_0001).Append(
                        new RootNode().Append(
                            new LookAt(playerBodyReference, aimReference)), 
                        new ClickInput(1.0f).Append(
                            new ShootAt(playerBodyReference, "aim.fireAnchor".Reference<Transform>(), bulletPrefab, muzzleFlashPrefab).Append(
                                new Delay(0.33f)))),
                    new MoveTo(new PackettedValue<NavMeshAgent>(), "player".Reference<Transform>(true), aimReference).Mask(0b_0010).Append(
                        new Delay(0.5f))));
        }

        void Start() => behaviourTree.Start(packet);
        void Update() => behaviourTree.Update(packet);
    }
}