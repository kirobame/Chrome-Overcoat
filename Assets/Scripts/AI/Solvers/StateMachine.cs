using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class StateMachine : Solver
    {
        #region Nested Types

        [Serializable]
        private class State : IAssignable<StateMachine>
        {
            object IAssignable.Value => Owner;
            public StateMachine Owner { get; private set; }
            
            public string Name => name.ToLower();

            [SerializeField] private string name;
            
            [SerializeReference] private ISolver[] solvers = new ISolver[0];
            [SerializeField] private Transition[] transitions;

            public void AssignTo(StateMachine owner) => Owner = owner;

            public void Build()
            {
                var agent = Owner.Recurse<IAgent>();
                
                foreach (var solver in solvers)
                {
                    solver.AssignTo(agent);
                    solver.Build();
                }
                
                foreach (var transition in transitions)
                {
                    var search = transition.To;
                    var state = default(State);
                    
                    try { state = Owner.States.First(candidate => candidate.Name == search); }
                    catch (Exception exception)
                    {
                        Debug.Log($"On agent : {agent} -> {exception}");
                        throw;
                    }

                    transition.AssignTo(this);
                    transition.Destination = state;
                }
            }
            
            public void Bootup() { foreach (var transition in transitions) transition.Bootup(); }
            public void Open()
            {
                foreach (var solver in solvers) solver.Bootup();
                foreach (var transition in transitions) transition.Open();
            }
            
            public State Evaluate()
            {
                foreach (var solver in solvers) solver.Evaluate();
                foreach (var transition in transitions)
                {
                    if (!transition.Evaluate()) continue;
                    return transition.Destination;
                }

                return this;
            }
            
            public void Close()
            {
                foreach (var solver in solvers) solver.Shutdown();
                foreach (var transition in transitions) transition.Close();
            }
            public void Shutdown() { foreach (var transition in transitions) transition.Shutdown(); }
        }

        [Serializable]
        private class Transition : IAssignable<State>
        {
            object IAssignable.Value => Owner;
            public State Owner { get; private set; }
            
            public State Destination { get; set; }
            public string To => to.ToLower();
            
            [SerializeField] private string to;
            [SerializeReference] private ICondition condition;

            public void AssignTo(State owner) => Owner = owner;
            
            public void Bootup() => condition.Bootup(GetPacket());
            public void Open()
            {
                var packet = GetPacket();
                
                condition.Open(packet);
                condition.Prepare(packet);
            }

            public bool Evaluate() => condition.Check(GetPacket());
            
            public void Close() => condition.Close(GetPacket());
            public void Shutdown() => condition.Shutdown(GetPacket());
            
            private Packet GetPacket() => Owner.Recurse<IAgent>(2).Identity.Packet;
        }
        #endregion

        private State Current => current;
        private IEnumerable<State> States => states;

        [SerializeField] private string entry;
        [SerializeField] private State[] states;

        private State current;

        public override void Build()
        {
            var search = entry.ToLower();
            current = states.FirstOrDefault(candidate => candidate.Name == search);

            foreach (var state in states)
            {
                state.AssignTo(this);
                state.Build();
            }
        }

        public override void Bootup() { foreach (var state in states) state.Bootup(); }

        public override void Evaluate() => Evaluate(0);
        private void Evaluate(int depth)
        {
            if (depth >= 7) return;
            
            var result = current.Evaluate();
            if (result == current) return;
            
            current.Close();
            result.Open();

            current = result;
            Evaluate(depth + 1);
        }
        
        public override void Shutdown(){ foreach (var state in states) state.Shutdown(); }
    }
}