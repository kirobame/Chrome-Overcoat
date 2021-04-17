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

        // TO DO : Add static extension methods to create IValue<T>
        
        void Awake()
        {
            board = new Blackboard();
            board.Set(aim, "aim");
            board.Set(fireAnchor, "aim.fireAnchor");
            
            packet = new Packet();
            packet.Set(navMesh);
            packet.Set(lineOfSight);
            packet.Set(board);

            behaviourTree = new RootNode();
            var conditionalNode = new CanSee();
            
            behaviourTree.Append(
                conditionalNode.Append(
                    new StopMoving().Mask(0b_0001).Append(
                        new RootNode().Append(
                            new LookAt()), 
                        new ClickInput(1.0f).Append(
                            new ShootAt(bulletPrefab, muzzleFlashPrefab).Append(
                                new Delay(0.33f)))),
                    new MoveTo().Mask(0b_0010).Append(
                        new Delay(0.5f))));
        }

        void Start() => behaviourTree.Start(packet);
        void Update() => behaviourTree.Update(packet);
    }
}