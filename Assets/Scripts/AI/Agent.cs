using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class Agent : MonoBehaviour, IAgent, ILifebound, IInstaller, IInjectable
    {
        private const string JEFF_TAG = "JEFF";

        IReadOnlyList<IValue> IInjectable.Injections => injections;
        protected List<IValue> injections;

        void IInjectable.PrepareInjection()
        {
            identity = new AnyValue<IIdentity>();
            lifetime = new AnyValue<Lifetime>();
            
            injections = new List<IValue>()
            {
                identity,
                lifetime
            };
        }

        //--------------------------------------------------------------------------------------------------------------/

        public event Action<ILifebound> onDestruction; 
        
        public event Action<IAgent> onSpawn;
        public event Action<IAgent> onDiscard;
        
        public event Action<IAgent, string, string> onStreamChange;

        //--------------------------------------------------------------------------------------------------------------/

        public IIdentity Identity => identity.Value;
        public Lifetime Lifetime => lifetime.Value;

        bool IAgent.IsActive => isOperational;
        private bool isOperational;

        bool IActive<ILifebound>.IsActive => true;
        
        public AgentDefinition Definition => definition;
        public string Stream => stream;

        public IGoal this[GoalDefinition definition] => goals.First(candidate => candidate.Definition == definition);
        public IEnumerable<IGoal> Goals => goals;

        [SerializeField] protected AgentDefinition definition;
        [SerializeField] protected string stream;
        
        [SerializeReference] protected IGoal[] goals = new IGoal[0];
        [SerializeReference] protected ISolver[] solvers = new ISolver[0];

        private bool hasBeenBootedUp;
        
        private IValue<IIdentity> identity;
        private IValue<Lifetime> lifetime;
        
        //--------------------------------------------------------------------------------------------------------------/

        protected virtual void Awake()
        {
            hasBeenBootedUp = false;
            
            foreach (var goal in goals) goal.AssignTo(this);
            foreach (var solver in solvers) solver.AssignTo(this);
        }
        protected virtual void OnDestroy() => onDestruction?.Invoke(this);
        
        public virtual void Bootup(byte code)
        {
            Hive.Register(this);
            if (!hasBeenBootedUp)
            {
                foreach (var solver in solvers)
                {
                    solver.Build();
                    solver.Bootup();
                }

                hasBeenBootedUp = true;
            }
            else foreach (var solver in solvers) solver.Bootup();
            foreach (var goal in goals) goal.Reset();

            //Debug.Log("Agent Bootup");
            isOperational = true;
            onSpawn?.Invoke(this);
        }
        public virtual void Shutdown(byte code)
        {
            foreach (var solver in solvers) solver.Shutdown();

            isOperational = false;
            onDiscard?.Invoke(this);
        }

        protected virtual void Update()
        {
            if (!hasBeenBootedUp) return;
            
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

            for (int i = index; i < goals.Length; i++)
            {
                this.goals[i] = goals[i - index];
                this.goals[i].Reset();
            }
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

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}