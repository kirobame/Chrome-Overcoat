using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public abstract class Node : IComparable<Node>
    {
        public virtual bool IsLocked
        {
            get => isLocked;
            set => isLocked = value;
        }
        protected bool isLocked;
        
        public bool IsDone { get; protected set; }

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

        public Node Parent { get; internal set; }
        
        public IReadOnlyList<Node> Childs => childs;
        private List<Node> childs = new List<Node>();
        
        //--------------------------------------------------------------------------------------------------------------/

        public abstract void Start(Packet packet);
        public abstract IEnumerable<Node> Update(Packet packet);

        public virtual void Shutdown()
        {
            OnShutdown();
            foreach (var child in childs) child.Shutdown();
        }
        protected virtual void OnShutdown() { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public void Insert(IEnumerable<Node> nodes)
        {
            var parents = new HashSet<Node>();
            var parent = Parent;
            
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.Parent;
            }
            
            foreach (var node in nodes)
            {
                if (parents.Contains(node)) continue;
                
                childs.Add(node);
                node.Parent = this;
            }
            
            childs.Sort();
        }

        public void Cut(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes) childs.Remove(node);
        }
        public void Cut(Predicate<Node> predicate) => childs.RemoveAll(predicate);

        //--------------------------------------------------------------------------------------------------------------/

        int IComparable<Node>.CompareTo(Node other) => priority.CompareTo(other.priority);
    }
}