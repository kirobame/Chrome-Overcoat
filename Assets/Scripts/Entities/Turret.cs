using Flux;
using Flux.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] private LineOfSight lineOfSight;
        
        [Space, SerializeField] private Transform aim;
        [SerializeField] private Transform fireAnchor;

        [Space, SerializeField] private GenericPoolable bulletPrefab;
        [SerializeField] private PoolableVfx muzzleFlashPrefab;
        
        private ITaskTree taskTree;
        private Blackboard board;
        private Packet packet;

        void Awake()
        {
            board = new Blackboard();
            board.Set((byte)2, "type");
            board.Set(aim, "aim");
            board.Set(fireAnchor, "aim.fireAnchor");
            
            packet = new Packet();
            packet.Set(lineOfSight);
            packet.Set(board);

            var playerBodyReference = "player.body".Reference<PhysicBody>(true);
            var fireAnchorReference = "aim.fireAnchor".Reference<Transform>();
            var aimReference = "aim".Reference<Transform>();
            
            taskTree = new RootNode();
            var conditionalNode = new CanSee(playerBodyReference, new PackettedValue<LineOfSight>());
            
            taskTree.Append(
                conditionalNode.Append(
                    new RootNode().Mask(0b_0001).Append(
                        new LookAt(playerBodyReference, aimReference)), 
                    new SimulatedClickInput(1.0f).Mask(0b_0001).Append(
                        new ComputeDirectionTo("shootDir", fireAnchorReference, playerBodyReference).Append(
                            new Shoot("shootDir".Reference<Vector3>(), fireAnchorReference, bulletPrefab, muzzleFlashPrefab).Append(
                                new Delay(0.33f))))),
                    new Delay(0.5f).Mask(0b_0010));
        }

        void Start() => taskTree.Start(packet);
        void Update() => taskTree.Update(packet);
    }
}