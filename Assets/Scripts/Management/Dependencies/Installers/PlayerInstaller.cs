using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PlayerInstaller : MonoBehaviour, IInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private Collider self;
        [FoldoutGroup("Values"), SerializeField] private Transform pivot;
        [FoldoutGroup("Values"), SerializeField] private Transform fireAnchor;
        [FoldoutGroup("Values"), SerializeField] private Animator animator;

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;
        
        public void InstallDependenciesOn(Packet packet)
        {
            packet.Set(animator);
            
            var board = packet.Get<IBlackboard>();
            board.Set(Refs.TYPE, (byte)10);
            board.Set(Refs.PIVOT, pivot);
            board.Set(Refs.FIREANCHOR, fireAnchor);
            board.Set(Refs.ROOT, self.transform);
            board.Set(Refs.COLLIDER, self);
            board.Set(PlayerRefs.CAN_SPRINT, new BusyBool());
        }
    }
}