using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class RootNode : Node, ITaskTree
    {
        public override bool IsLocked
        {
            get => isLocked || UpdatedNodes.Any(node => node.IsLocked);
            set => isLocked = value;
        }
        public override bool IsDone => branches.All(branch => branch.IsDone);

        public IEnumerable<INode> UpdatedNodes => branches.SelectMany(branch => branch.Nodes);
        public IEnumerable<Branch> Branches => branches;

        protected List<Branch> branches = new List<Branch>();
        protected List<ICommand> commands = new List<ICommand>();
        
        //--------------------------------------------------------------------------------------------------------------/

        public override void Prepare(Packet packet)
        {
            base.Prepare(packet);

            var keys = new List<int>();
            foreach (var branch in branches)
            {
                keys.Add(branch.Key);
                branch.Reset();
            }
            
            foreach (var child in Children)
            {
                if ((Output | child.Input) != Output) continue;
                
                child.Prepare(packet);
                keys.Remove(child.Input);

                var index = branches.FindIndex(branch => branch.Key == child.Input);
                if (index == -1) branches.Add(new Branch(this, child.Input, child));
                else branches[index].Add(child);
            }

            branches.RemoveAll(branch => keys.Contains(branch.Key));
        }

        public override IEnumerable<INode> Use(Packet packet)
        {
            if (IsDone) Prepare(packet);
            
            foreach (var branch in branches) branch.Update(packet);
            OnUse(packet);

            return null;
        }
        protected virtual void OnUse(Packet packet) { }

        public override void Shutdown(Packet packet)
        {
            foreach (var branch in branches)
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
        protected void RemoveOutputChannel(Packet packet, int value) => ChangeOutputMask(packet, output ^ (output & value));

        void ITaskTree.ChangeOutputMask(Packet packet, int value) => ChangeOutputMask(packet, value);
        protected void ChangeOutputMask(Packet packet, int value)
        {
            var change = output ^ value;
            var subtraction = output & change;

            foreach (var branch in branches)
            {
                if ((subtraction | branch.Key) != subtraction) continue;
                
                foreach (var node in branch.All) node.Close(packet);
                branch.Reset();
            }

            output = value;
        }
    }
}