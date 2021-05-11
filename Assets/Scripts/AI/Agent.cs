using System;
using System.Collections;
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
        
        void Interrupt(EventArgs args);
        
        void AddGoals(params IGoal[] goals);
        void RemoveGoals(GoalDefinition query);

        void Tag(string partialStream);
        void Erase(string partialStream);
    }
    
    public class Agent : MonoBehaviour, IAgent, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        //--------------------------------------------------------------------------------------------------------------/

        public event Action<IAgent> onSpawn;
        public event Action<IAgent> onDiscard;
        
        public event Action<IAgent, string, string> onStreamChange;

        //--------------------------------------------------------------------------------------------------------------/

        public IIdentity Identity => identity.Value;
        public bool IsActive { get; protected set; }
        
        public AgentDefinition Definition => definition;
        public string Stream => stream;

        public IEnumerable<IGoal> Goals => goals;

        [SerializeField] protected AgentDefinition definition;
        [SerializeField] protected string stream;
        
        [SerializeReference] protected IGoal[] goals = new IGoal[0];
        [SerializeReference] protected ISolver[] solvers = new ISolver[0];

        private IValue<IIdentity> identity;
        
        //--------------------------------------------------------------------------------------------------------------/

        protected virtual void Awake()
        {
            identity = new AnyValue<IIdentity>();
            injections = new List<IValue>() { identity };
            
            foreach (var solver in solvers) solver.Build();
        }

        protected virtual void OnEnable()
        {
            Hive.Register(this);
            foreach (var solver in solvers) solver.Bootup();
            
            onSpawn?.Invoke(this);
        }
        protected virtual void OnDisable()
        {
            foreach (var solver in solvers) solver.Shutdown();
            onDiscard?.Invoke(this);
        }

        protected virtual void Update()
        {
            foreach (var solver in solvers) solver.Evaluate();
            foreach (var goal in goals)
            {
                if (!goal.IsDirty || goal.IsAccomplished) continue;
                
                goal.Evaluate();
                goal.IsDirty = false;
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Interrupt(EventArgs args)
        {
            // TO DO...
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void AddGoals(params IGoal[] goals)
        {
            var index = this.goals.Length;
            Array.Resize(ref this.goals, this.goals.Length + goals.Length);

            for (int i = index; i < goals.Length; i++) this.goals[i] = goals[i - index];
        }
        public void RemoveGoals(GoalDefinition query)
        {
            var indices = new List<int>();
            for (var i = 0; i < goals.Length; i++)
            {
                if ((goals[i].Definition | query) == query) continue;
                indices.Add(i);
            }

            var copy = new IGoal[indices.Count];
            for (int i = 0; i < copy.Length; i++) copy[i] = goals[indices[i]];

            Array.Clear(goals, 0, goals.Length);
            goals = copy;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void Tag(string partialStream)
        {
            var tags = partialStream.Split(Hive.STREAM_SEPARATOR);
            var addition = string.Empty;
            foreach (var tag in tags)
            {
                if (stream.Contains(tag)) continue;
                addition += $"{tag}{Hive.STREAM_SEPARATOR}";
            }

            if (addition == string.Empty) return;
            
            addition = addition.Remove(addition.Length - 1, 1);
            if (stream == string.Empty) stream = addition;
            else stream += $"{Hive.STREAM_SEPARATOR}{addition}";
            
            onStreamChange?.Invoke(this, addition, string.Empty);
        }
        public void Erase(string partialStream)
        {
            var tags = partialStream.Split(Hive.STREAM_SEPARATOR);
            var removal = string.Empty;
            foreach (var tag in tags)
            {
                var index = stream.IndexOf(tag, StringComparison.Ordinal);
                if (index == -1) continue;
                
                removal += $"{tag}{Hive.STREAM_SEPARATOR}";
                if (index + tag.Length < stream.Length) stream = stream.Remove(index, tag.Length + 1);
                else stream = stream.Remove(index, tag.Length);
            }

            if (removal == string.Empty) return;
            
            removal = removal.Remove(removal.Length - 1, 1);
            onStreamChange?.Invoke(this, string.Empty, removal);
        }
    }

    public interface ISolver
    {
        IAgent Owner { get; }

        void Build();

        void Bootup();
        void Evaluate();
        void Shutdown();
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
        
        bool IsDirty { get; set; }
        bool IsAccomplished { get; }
        
        void Reset();
        void Evaluate();

        void Accomplish();
    }
}