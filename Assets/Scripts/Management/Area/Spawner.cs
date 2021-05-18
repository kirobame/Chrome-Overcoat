using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public class Spawner : MonoBehaviour, IInstaller, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            area = new AnyValue<Area>();
            injections = new IValue[] { area };
        }

        //--------------------------------------------------------------------------------------------------------------/

        [SerializeField] private SpawnLocations value;
        [SerializeField] private bool hasCachedEnemy;
        
        [ShowIf("hasCachedEnemy"), SerializeField] private PoolableAgent agentPrefab;
        [ShowIf("hasCachedEnemy"), SerializeReference] private IAgentModification[] modifications = new IAgentModification[0];

        private IValue<Area> area;
        
        //--------------------------------------------------------------------------------------------------------------/

        void Start()
        {
            if (!hasCachedEnemy) return;
            
            var agentPool = Repository.Get<AgentPool>(Pool.Agent);
            var agentInstance = agentPool.RequestSingle(agentPrefab);
            
            var link = agentInstance.Identity.Packet.Get<AreaLink>();
            link.Set(area.Value);

            foreach (var modification in modifications) modification.Modify(agentInstance);
            Spawn(agentInstance);
        }
        
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