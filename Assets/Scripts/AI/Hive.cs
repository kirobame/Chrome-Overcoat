using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public static class Hive
    {
        public const char STREAM_SEPARATOR = '-';
        private const string JEFF_TAG = "JEFF";

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
        public static TAgent[] Query<TAgent>(string inclusion) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, string.Empty, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, string exclusion) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, exclusion, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, Predicate<TAgent> predicate, string exclusion) where TAgent : IAgent => Query<TAgent>(inclusion, predicate, exclusion, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, string exclusion, Predicate<TAgent> predicate) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, exclusion, predicate);
        public static TAgent[] Query<TAgent>(string inclusion, Predicate<TAgent> inclusionPredicate, string exclusion, Predicate<TAgent> exclusionPredicate) where TAgent : IAgent
        {
            var inclusionTags = inclusion.Split(STREAM_SEPARATOR);
            if (inclusionTags.Any(tag => !repository.ContainsKey(tag)))
            {
                //Debug.Log($"[QUERY] -> One or more of the tag of the inclusion [{inclusion}] stream could not be found!");
                return Array.Empty<TAgent>();
            }

            var result = repository[inclusionTags[0]].OfType<TAgent>().Where(agent => inclusionPredicate(agent));
            var includedAgents = new HashSet<TAgent>(result);
            //Debug.Log($"[QUERY] -> For tag [{inclusionTags[0]}] : {result.Count()} results were found!");

            for (var i = 1; i < inclusionTags.Length; i++)
            {
                result = repository[inclusionTags[i]].OfType<TAgent>().Where(agent => inclusionPredicate(agent));
                includedAgents.IntersectWith(result);

                //Debug.Log($"[QUERY] -> For tag [{inclusionTags[i]}] : {result.Count()} results were found!");
            }

            if (exclusion == string.Empty)
            {
                //Debug.Log($"[QUERY] -> Skipping exclusion!");
                return includedAgents.ToArray();
            }

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