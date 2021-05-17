using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class AgentInstaller : MonoBehaviour, IInstaller
    {
        [FoldoutGroup("Values"), SerializeField] private NavMeshAgent navMesh;
        [FoldoutGroup("Values"), SerializeField] private LineOfSight lineOfSight;
        [FoldoutGroup("Values"), SerializeField] private Transform pivot;
        [FoldoutGroup("Values"), SerializeField] private Transform fireAnchor;
        [FoldoutGroup("Values"), SerializeField] private new Collider collider;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        int IInstaller.Priority => 1;
        
        void IInstaller.InstallDependenciesOn(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            
            board.Set(AgentRefs.NAV, navMesh);
            board.Set(AgentRefs.LINE_OF_SIGHT, lineOfSight);
            board.Set(Refs.PIVOT, pivot);
            board.Set(Refs.FIREANCHOR, fireAnchor);
            board.Set(Refs.COLLIDER, collider);
        }
    }
}