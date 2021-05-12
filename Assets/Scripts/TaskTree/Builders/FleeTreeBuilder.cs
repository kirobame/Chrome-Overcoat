using System;
using UnityEngine.AI;

namespace Chrome
{
    [Serializable]
    public class FleeTreeBuilder : ITreeBuilder
    {
        public ITaskTree Build()
        {
            var navReference = AgentRefs.NAV.Reference<NavMeshAgent>();
            
            return new RootNode().Append(
                new StopMoving(navReference).Append(
                    new Print("FLEEING").Append(
                        new Delay(1.0f))));
        }
    }
}