using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class Spawner : MonoBehaviour, IInstaller
    {
        [SerializeField] private SpawnLocations value;

        public void Spawn(Agent agent)
        {
            agent.Lifetime.Begin();
            
            var board = agent.Identity.Packet.Get<IBlackboard>();
            var nav = board.Get<NavMeshAgent>(AgentRefs.NAV);
            nav.Warp(transform.position);
        }
        
        //--------------------------------------------------------------------------------------------------------------/ 

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.SetKeyValuePair(value, this);
    }
}