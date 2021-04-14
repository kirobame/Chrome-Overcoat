using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class BoardSnapshot
    {
        public BoardSnapshot(IEnumerable<object> values) => this.values = values.ToArray();
        
        public IReadOnlyList<object> Values => values;
        private object[] values;
    }
    
    public class Board
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        public BoardSnapshot Save() => new BoardSnapshot(map.Values);
        public void Load(BoardSnapshot snapshot)
        {
            var keys = new HashSet<Type>(map.Keys);
            foreach (var value in snapshot.Values)
            {
                var type = value.GetType();
                if (!map.ContainsKey(type)) continue;

                map[type] = value;
                keys.Remove(type);
            }

            foreach (var key in keys) map.Remove(key);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public bool TryGet<T>(out T value)
        {
            if (map.TryGetValue(typeof(T), out var rawValue))
            {
                value = (T)rawValue;
                return true;
            }

            value = default;
            return false;
        }
        public void Set<T>(T value)
        {
            if (map.ContainsKey(typeof(T))) map[typeof(T)] = value;
            else map.Add(typeof(T), value);
        }
    }

    [Serializable]
    public abstract class Node : IComparable<Node>
    {
        public virtual bool IsLocked
        {
            get => isLocked;
            set => isLocked = value;
        }
        protected bool isLocked;

        public int Input => input;
        [SerializeField] protected int input;
        
        public int Output => output;
        [SerializeField] protected int output;
        
        public int Priority => priority;
        [SerializeField] protected int priority;

        public Node Parent { get; internal set; }
        
        public IReadOnlyList<Node> Childs => childs;
        private List<Node> childs = new List<Node>();
        
        //--------------------------------------------------------------------------------------------------------------/

        public abstract void Start(Board board);
        public abstract IEnumerable<Node> Update(Board board);
        
        public virtual void Cleanup() { }
        
        //--------------------------------------------------------------------------------------------------------------/

        public void Append(IEnumerable<Node> nodes)
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

    public class RootNode : Node
    {
        private List<Node> updatedNodes = new List<Node>();
        
        public override void Start(Board board)
        {
            updatedNodes.Clear();
            foreach (var child in Childs)
            {
                if ((output | child.Input) != Output) continue;
                updatedNodes.Add(child);
            }
            
            OnStart(board);
        }
        protected virtual void OnStart(Board board) { }

        public override IEnumerable<Node> Update(Board board)
        {
            var previousOutput = output;
            OnUpdate(board);

            if (previousOutput != output)
            {
                // TO DO : Reset only new paths & keep already valid one at their current advancement
            }

            for (var i = 0; i < updatedNodes.Count; i++) UpdateNodeAt(i, board);
            return null;
        }
        protected virtual void OnUpdate(Board board) { }
        
        //--------------------------------------------------------------------------------------------------------------/

        private int UpdateNodeAt(int index, Board board)
        {
            var result = updatedNodes[index].Update(board);
            if (result == null) return index;
            
            updatedNodes.RemoveAt(index);

            var count = result.Count();
            if (count == 0) return index;
            
            updatedNodes.InsertRange(index, result);
            for (var i = 0; i < count; i++) index = UpdateNodeAt(index + i, board);

            return index;
        }
    }
    
    public abstract class ProxyNode : Node
    {
        public bool IsDone { get; protected set; }
        
        public override void Start(Board board)
        {
            IsDone = false;
            OnStart(board);
        }
        protected virtual void OnStart(Board board) { }
        
        public override IEnumerable<Node> Update(Board board)
        {
            OnUpdate(board);
            if (!IsDone) return null;

            var selection = new List<Node>();
            foreach (var child in Childs)
            {
                if ((output | child.Input) != Output) continue;
                selection.Add(child);
            }

            return selection;
        }
        protected virtual void OnUpdate(Board board) { }

        protected void End(int output)
        {
            IsDone = true;
            this.output = output;
        }
    }
}