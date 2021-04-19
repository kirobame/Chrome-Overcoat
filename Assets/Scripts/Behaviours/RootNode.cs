using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class RootNode : Node
    {
        public override bool IsLocked
        {
            get => isLocked || UpdatedNodes.Any(node => node.IsLocked);
            set => isLocked = value;
        }

        public IEnumerable<INode> UpdatedNodes => updatedNodes.SelectMany(kvp => kvp.Value);

        protected Dictionary<int, List<INode>> updatedNodes = new Dictionary<int, List<INode>>();
        protected Queue<ICommand> commands = new Queue<ICommand>();
        
        //--------------------------------------------------------------------------------------------------------------/

        public override void Start(Packet packet)
        {
            base.Start(packet);
            
            updatedNodes.Clear();
            foreach (var child in Children)
            {
                if ((Output | child.Input) != Output) continue;
                
                child.Start(packet);
                
                if (updatedNodes.TryGetValue(child.Input, out var list)) list.Add(child);
                else updatedNodes.Add(child.Input, new List<INode>() {child} );
            }
        }

        public override IEnumerable<INode> Update(Packet packet)
        {
            if (IsDone) Start(packet);
            
            UpdateCachedNodes(packet);
            OnUpdate(packet);

            if (CanBreak())
            {
                IsLocked = false;
                IsDone = true;
            }

            return null;
        }
        protected void UpdateCachedNodes(Packet packet)
        {
            foreach (var kvp in updatedNodes)
            {
                for (var i = 0; i < updatedNodes.Count; i++)
                {
                    i = UpdateNodeAt(kvp.Key, i, packet);
                    if (i == -1) return;
                }
            }
        }
        protected virtual void OnUpdate(Packet packet) { }

        // Go recursively from the childs rather than using a dictionary
        // Will allow to shutdown even when no currents are up for a given branch
        // Need to take into account nodes that have no still been processed once : Use NodeState.Inactive to break
        public override void OnClose(Packet packet) { foreach (var node in UpdatedNodes) CloseFrom(packet, node); }
        private void CloseFrom(Packet packet, INode node)
        {
            var current = node;
            current.Close(packet);

            while (current.Parent != null && current.Parent != this && current.Parent.State == NodeState.Active)
            {
                current = node.Parent;
                current.Close(packet);
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void Order(ICommand command) => commands.Enqueue(command);
        protected void HandleCommand(ICommand command, Packet packet)
        {
            switch (command)
            {
                case PulseCommand pulse:

                    if (IsUpdating(pulse.Target))
                    {
                        pulse.Target.Start(packet);
                        break;
                    }
                    
                    if (!pulse.Target.IsChildOf(this)) break;

                    bool foundMatch = false;
                    KeyValuePair<int, List<INode>> match;
                    
                    foreach (var kvp in updatedNodes)
                    {
                        var list = kvp.Value;
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (!list[i].IsChildOf(pulse.Target)) continue;

                            foundMatch = true;
                            match = kvp;
                            
                            list.RemoveAt(i);
                            i--;
                        }
                    }
                    
                    if (foundMatch)
                    {
                        pulse.Target.Start(packet);
                        updatedNodes[match.Key].Add(pulse.Target);
                    }
                    break;
                
                case CloseCommand close:
                    
                    Close(packet);
                    break;
            }
        }

        //--------------------------------------------------------------------------------------------------------------/

        protected void ChangeOutput(Packet packet, int value)
        {
            var change = output ^ value;
            var subtraction = output & change;
            
            foreach (var kvp in updatedNodes)
            {
                if ((subtraction | kvp.Key) != subtraction) continue;
                foreach (var node in kvp.Value) CloseFrom(packet, node);
            }

            output = value;
        }
        
        public bool IsUpdating(INode node) => UpdatedNodes.Any(value => value == node);
        protected bool CanBreak() => updatedNodes.Count == 0 || UpdatedNodes.All(node => node.IsDone);

        protected int UpdateNodeAt(int key, int index, Packet packet)
        {
            var snapshot = packet.Save();
            var result = updatedNodes[key][index].Update(packet);
            
            if (!IsLocked)
            {
                while (commands.Count > 0)
                {
                    var command = commands.Dequeue();
                    HandleCommand(command, packet);
                }
            }

            if (IsDone) return -1;
            
            if (result == null) return index;
            updatedNodes[key].RemoveAt(index);

            var count = result.Count();
            if (count == 0)
            {
                if (updatedNodes[key].Count == 0) updatedNodes.Remove(key);
                return index - 1;
            }
            
            updatedNodes[key].InsertRange(index, result);
            for (var i = 0; i < count; i++) index = UpdateNodeAt(key, index + i, packet);

            packet.Load(snapshot);
            return index;
        }
    }
}