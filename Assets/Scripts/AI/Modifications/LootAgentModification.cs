using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class LootAgentModification : IAgentModification
    {
        [SerializeField] private GenericPoolable lootPrefab;
        
        public void Modify(Agent agent)
        {
            var modificationPool = Repository.Get<GenericPool>(Pool.Modification);
            var lootInstance = modificationPool.CastSingle<LootControl>(lootPrefab);
            
            lootInstance.AssignTo(agent);
            lootInstance.transform.SetParent(agent.Identity.Transform);
            lootInstance.transform.RebootLocally();

            var lifetime = agent.Identity.Packet.Get<Lifetime>();
            lifetime.AddBound(lootInstance);
        }
    }
}