using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class Node : INode
    {
        public virtual bool IsLocked
        {
            get => isLocked;
            set => isLocked = value;
        }
        protected bool isLocked;

        public abstract bool IsDone { get; }
        public NodeState State { get; protected set; }
        
        public INode Parent { get; set; }

        public int Priority
        {
            get => priority;
            set => priority = value;
        }
        [SerializeField] protected int priority;
        
        public int Input
        {
            get => input;
            set => input = value;
        }
        [SerializeField] protected int input;
        
        public int Output => output;
        [SerializeField] protected int output;
        
        public IReadOnlyList<INode> Children => children;
        private List<INode> children = new List<INode>();

        //--------------------------------------------------------------------------------------------------------------/

        public virtual void Bootup(Packet packet)
        {
            IsLocked = false;
            State = NodeState.Inactive;
            
            foreach (var child in children) child.Bootup(packet);
            OnBootup(packet);
        }
        protected virtual void OnBootup(Packet packet) { }

        protected virtual void Open(Packet packet) { }

        public virtual void Prepare(Packet packet)
        {
            if (State == NodeState.Inactive)
            {
                Open(packet);
                State = NodeState.Active;
            }
            
            OnPrepare(packet);
        }
        protected virtual void OnPrepare(Packet packet) { }
        
        public abstract IEnumerable<INode> Use(Packet packet);

        public virtual void Close(Packet packet)
        {
            IsLocked = false;
            State = NodeState.Inactive;
            
            if (Parent != null && Parent.State == NodeState.Active) Parent.Close(packet);
            OnClose(packet);
        }
        public virtual void OnClose(Packet packet) { }
        
        public virtual void Shutdown(Packet packet)
        {
            State = NodeState.Shutdown;
            
            foreach (var child in children) child.Shutdown(packet);
            OnShutdown(packet);
        }
        protected virtual void OnShutdown(Packet packet) { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public void Insert(IEnumerable<INode> nodes)
        {
            var parents = new HashSet<INode>();
            var parent = Parent;
            
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.Parent;
            }
            
            foreach (var node in nodes)
            {
                if (parents.Contains(node)) continue;
                
                children.Add(node);
                node.Parent = this;
            }
            
            children.Sort();
        }

        public void Cut(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes) children.Remove(node);
        }
        public void Cut(Predicate<INode> predicate) => children.RemoveAll(predicate);

        //--------------------------------------------------------------------------------------------------------------/

        int IComparable<INode>.CompareTo(INode other) => priority.CompareTo(other.Priority);
    }
}