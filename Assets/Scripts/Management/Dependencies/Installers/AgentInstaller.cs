using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class AgentInstaller : MonoBehaviour, IInstaller
    {
        [FoldoutGroup("Values"), SerializeField] protected NavMeshAgent navMesh;
        [FoldoutGroup("Values"), SerializeField] protected Transform pivot;
        [FoldoutGroup("Values"), SerializeField] protected Transform view;
        [FoldoutGroup("Values"), SerializeField] protected Transform fireAnchor;
        [FoldoutGroup("Values"), SerializeField] protected new Renderer renderer;
        [FoldoutGroup("Values"), SerializeField] protected new Collider collider;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;
        
        void IInstaller.InstallDependenciesOn(Packet packet)
        {
            var board = packet.Get<IBlackboard>();

            board.Set(Refs.ROOT, collider.transform);
            board.Set(Refs.SHOOT_DIRECTION, Vector3.zero);
            board.Set(AgentRefs.NAV, navMesh);
            board.Set(Refs.PIVOT, pivot);
            board.Set(Refs.VIEW, view);
            board.Set(Refs.FIREANCHOR, fireAnchor);
            board.Set(Refs.RENDERER, renderer);
            board.Set(Refs.COLLIDER, collider);
            
            InstallDependenciesOn(packet, board);
        }
        protected virtual void InstallDependenciesOn(Packet packet, IBlackboard board) { }
    }
}