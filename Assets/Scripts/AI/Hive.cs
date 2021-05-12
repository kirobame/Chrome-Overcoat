using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public static class Hive
    {
        public const char STREAM_SEPARATOR = '-';
        
        private static Dictionary<string, List<IAgent>> repository;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootup() => repository = new Dictionary<string, List<IAgent>>();

        //--------------------------------------------------------------------------------------------------------------/
        
        public static void Register(IAgent agent)
        {
            AddStreamFor(agent, agent.Stream);

            agent.onStreamChange += OnAgentStreamChange;
            agent.onDiscard += OnAgentDiscard;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static TAgent[] Query<TAgent>(string inclusion, string exclusion) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, exclusion, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, Predicate<TAgent> predicate, string exclusion) where TAgent : IAgent => Query<TAgent>(inclusion, predicate, exclusion, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, string exclusion, Predicate<TAgent> predicate) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, exclusion, predicate);
        public static TAgent[] Query<TAgent>(string inclusion, Predicate<TAgent> inclusionPredicate, string exclusion, Predicate<TAgent> exclusionPredicate) where TAgent : IAgent
        {
            var inclusionTags = inclusion.Split(STREAM_SEPARATOR);
            if (inclusionTags.Any(tag => !repository.ContainsKey(tag))) return Array.Empty<TAgent>();
            
            var includedAgents = new HashSet<TAgent>();
            foreach (var tag in inclusionTags) includedAgents.IntersectWith(repository[tag].OfType<TAgent>().Where(agent => inclusionPredicate(agent)));

            var copy = new TAgent[includedAgents.Count];
            includedAgents.CopyTo(copy);

            var exclusionTags = exclusion.Split(STREAM_SEPARATOR);
            foreach (var agent in copy)
            {
                if (exclusionTags.Any(tag => agent.Stream.Contains(tag)) || exclusionPredicate(agent))
                {
                    includedAgents.Remove(agent);
                    continue;
                }
            }
            
            return includedAgents.ToArray();
        }

        //--------------------------------------------------------------------------------------------------------------/

        private static void AddStreamFor(IAgent agent, string stream)
        {
            if (stream == string.Empty) return;
            
            var tags = stream.Split(STREAM_SEPARATOR);
            foreach (var tag in tags)
            {
                if (repository.TryGetValue(tag, out var list))
                {
                    if (list.Contains(agent)) continue;
                    list.Add(agent);
                } 
                else repository.Add(tag, new List<IAgent>() { agent }); 
            }
        }
        private static void RemoveStreamFor(IAgent agent, string stream)
        {
            if (stream == string.Empty) return;
            
            var tags = stream.Split(STREAM_SEPARATOR);
            foreach (var tag in tags)
            {
                if (!repository.TryGetValue(tag, out var list)) continue;

                list.Remove(agent);
                if (!list.Any()) repository.Remove(tag);
            }
        }
        
        static void OnAgentStreamChange(IAgent agent, string addition, string removal)
        {
            AddStreamFor(agent, addition);
            RemoveStreamFor(agent, removal);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        static void OnAgentDiscard(IAgent agent)
        {
            agent.onStreamChange -= OnAgentStreamChange;
            agent.onDiscard -= OnAgentDiscard;

            RemoveStreamFor(agent, agent.Stream);
        }
    }
}