using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class RootNode : Node, ITaskTree
    {
        #region Nested Types

        #endregion
        
        public override bool IsLocked
        {
            get => isLocked || UpdatedNodes.Any(node => node.IsLocked);
            set => isLocked = value;
        }
        public override bool IsDone => Branches.All(branch => branch.IsDone);

        public IEnumerable<INode> UpdatedNodes => branchRegistry.SelectMany(kvp => kvp.Value.Nodes);
        public IEnumerable<Branch> Branches => branchRegistry.Values;

        public IReadOnlyDictionary<int, Branch> BranchRegistry => branchRegistry;
        protected Dictionary<int, Branch> branchRegistry = new Dictionary<int, Branch>();
        
        protected List<ICommand> commands = new List<ICommand>();
        
        //--------------------------------------------------------------------------------------------------------------/

        public override void Start(Packet packet)
        {
            base.Start(packet);

            var keys = new List<int>();
            foreach (var branch in Branches)
            {
                keys.Add(branch.Key);
                branch.Reset();
            }
            
            foreach (var child in Children)
            {
                if ((Output | child.Input) != Output) continue;
                
                child.Start(packet);
                keys.Remove(child.Input);

                if (!branchRegistry.ContainsKey(child.Input)) branchRegistry.Add(child.Input, new Branch(this, child.Input));
                branchRegistry[child.Input].Add(child);
            }

            foreach (var key in keys) branchRegistry.Remove(key);
        }

        public override IEnumerable<INode> Update(Packet packet)
        {
            if (IsDone) Start(packet);
            
            foreach (var branch in Branches) branch.Update(packet);
            OnUpdate(packet);

            return null;
        }
        protected virtual void OnUpdate(Packet packet) { }

        public override void Shutdown(Packet packet)
        {
            foreach (var branch in Branches)
            {
                foreach (var node in branch.All) node.Close(packet);
                branch.Reset();
            }
            
            base.Shutdown(packet);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public bool IsUpdating(INode node) => UpdatedNodes.Any(value => value == node);
        
        public void Command(Packet packet, ICommand command)
        {
            var state = command.IsReady(packet, this);
            
            if (state == null) return;
            else if (state == true) command.Execute(packet, this);
            else commands.Add(command);
        }

        void ITaskTree.ActualizeCommands(Packet packet) => ActualizeCommands(packet);
        protected void ActualizeCommands(Packet packet)
        {
            for (var i = 0; i < commands.Count; i++)
            {
                var state = commands[i].IsReady(packet, this);
                if (state == null)
                {
                    commands.RemoveAt(i);
                    i--;
                }
                else if (state == true)
                {
                    commands[i].Execute(packet, this);
                    commands.RemoveAt(i);
                    i--;
                }
            }
        }
        
        void ITaskTree.AddOutputChannel(Packet packet, int value) => AddOutputChannel(packet, value);
        protected void AddOutputChannel(Packet packet, int value) => ChangeOutputMask(packet, output | value);
        
        void ITaskTree.RemoveOutputChannel(Packet packet, int value) => RemoveOutputChannel(packet, value);
        protected void RemoveOutputChannel(Packet packet, int value) => ChangeOutputMask(packet, output ^ value);

        void ITaskTree.ChangeOutputMask(Packet packet, int value) => ChangeOutputMask(packet, value);
        protected void ChangeOutputMask(Packet packet, int value)
        {
            var change = output ^ value;
            var subtraction = output & change;

            foreach (var branch in Branches)
            {
                if ((subtraction | branch.Key) != subtraction) continue;
                
                foreach (var node in branch.All) node.Close(packet);
                branch.Reset();
            }

            output = value;
        }
    }
}