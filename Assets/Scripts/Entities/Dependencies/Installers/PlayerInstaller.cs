using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PlayerInstaller : MonoBehaviour, IInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private Collider self;
        [FoldoutGroup("Values"), SerializeField] private Transform raypoint;
        [FoldoutGroup("Values"), SerializeField] private Transform firepoint;
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;
        
        public void InstallDependenciesOn(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            
            board.Set("type", (byte)10);
            board.Set("canSprint", new BusyBool());
            board.Set("view", raypoint);
            board.Set("view.fireAnchor", firepoint);
            board.Set("self", self.transform);
            board.Set("self.collider", self);
        }
    }
}