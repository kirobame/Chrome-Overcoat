using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public static class Network
    {
        public const char SEPARATOR = '-';
        
        private static Dictionary<string, List<IAgent>> repository;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootup() => repository = new Dictionary<string, List<IAgent>>();

        public static void Register(IAgent agent)
        {
            
        }

        public static TAgent[] Query<TAgent>(string inclusion, string exclusion) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, exclusion, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, Predicate<TAgent> predicate, string exclusion) where TAgent : IAgent => Query<TAgent>(inclusion, predicate, exclusion, agent => false);
        public static TAgent[] Query<TAgent>(string inclusion, string exclusion, Predicate<TAgent> predicate) where TAgent : IAgent => Query<TAgent>(inclusion, agent => true, exclusion, predicate);
        public static TAgent[] Query<TAgent>(string inclusion, Predicate<TAgent> inclusionPredicate, string exclusion, Predicate<TAgent> exclusionPredicate) where TAgent : IAgent
        {
            var inclusionTags = inclusion.Split(SEPARATOR);
            if (inclusionTags.Any(tag => !repository.ContainsKey(tag))) return Array.Empty<TAgent>();
            
            var includedAgents = new HashSet<TAgent>();
            foreach (var tag in inclusionTags) includedAgents.IntersectWith(repository[tag].OfType<TAgent>().Where(agent => inclusionPredicate(agent)));

            var copy = new TAgent[includedAgents.Count];
            includedAgents.CopyTo(copy);

            var exclusionTags = exclusion.Split(SEPARATOR);
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
        
        static void OnAgentDiscard(IAgent agent)
        {
            
        }
        
        static void OnAgentStreamChange(IAgent agent, string additions, string removals)
        {
            
        }
    }
    
    [Flags]
    public enum AgentDefinition : short
    {
        None = 0,
        
        Peon = 1,
        Guard = 2
    }
    
    public interface IAgent
    {  
        event Action<IAgent> onSpawn;
        event Action<IAgent> onDiscard;
        event Action<IAgent,string,string> onStreamChange;

        IIdentity Identity { get; }
        
        bool IsActive { get; }
        AgentDefinition Definition { get; }
        string Stream { get; }
        
        IEnumerable<IGoal> Goals { get; }
        
        void AddGoals(params IGoal[] goals);
        void RemoveGoals(GoalDefinition query);
        void EvaluateGoals(GoalDefinition query);

        void Bootup();
        void Interrupt(EventArgs args);

        void Tag(string partialStream);
        void Erase(string partialStream);
    }

    [Flags]
    public enum GoalDefinition : short
    {
        None = 0,
        
        Attack = 1,
        Flee = 2
    }
    
    public interface IGoal
    {
        IAgent Owner { get; }
        GoalDefinition Definition { get; }
        
        bool IsAccomplished { get; }
        
        void Reset();
        void Evaluate();
    }
}