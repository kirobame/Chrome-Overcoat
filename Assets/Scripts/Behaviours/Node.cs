using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public enum NodeState
    {
        Shutdown,
        Inactive,
        Active,
    }
    
    public interface INode : IComparable<INode>
    {
        bool IsDone { get; }
        bool IsLocked { get; }
        NodeState State { get; }
        
        int Priority { get; }
        int Input { get; set; }
        int Output { get; }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        INode Parent { get; set; }
        IReadOnlyList<INode> Children { get; }

        void Bootup(Packet packet);

        void Start(Packet packet);
        IEnumerable<INode> Update(Packet packet);

        void Close(Packet packet);
        void Shutdown(Packet packet);
        
        //--------------------------------------------------------------------------------------------------------------/

        void Insert(IEnumerable<INode> nodes);
        
        void Cut(IEnumerable<INode> nodes);
        void Cut(Predicate<INode> predicate);
    }
    
    [Serializable]
    public abstract class Node : INode
    {
        public virtual bool IsLocked
        {
            get => isLocked;
            set => isLocked = value;
        }
        protected bool isLocked;

        public bool IsDone { get; protected set; }
        public NodeState State { get; protected set; }

        public int Input
        {
            get => input;
            set => input = value;
        }
        [SerializeField] protected int input;
        
        public int Output => output;
        [SerializeField] protected int output;
        
        public int Priority => priority;
        [SerializeField] protected int priority;

        public INode Parent { get; set; }
        
        public IReadOnlyList<INode> Children => children;
        private List<INode> children = new List<INode>();

        //--------------------------------------------------------------------------------------------------------------/

        public virtual void Bootup(Packet packet)
        {
            IsDone = true;
            IsLocked = false;
            State = NodeState.Inactive;
            
            foreach (var child in children) child.Bootup(packet);
            OnBootup(packet);
        }
        protected virtual void OnBootup(Packet packet) { }

        protected virtual void Open(Packet packet) { }

        public virtual void Start(Packet packet)
        {
            IsDone = false;
            
            if (State == NodeState.Inactive)
            {
                Open(packet);
                State = NodeState.Active;
            }
            
            OnStart(packet);
        }
        protected virtual void OnStart(Packet packet) { }
        
        public abstract IEnumerable<INode> Update(Packet packet);

        public virtual void Close(Packet packet)
        {
            IsDone = true;
            State = NodeState.Inactive;
            
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