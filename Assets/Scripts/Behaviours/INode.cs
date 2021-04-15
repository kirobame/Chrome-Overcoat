using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Chrome
{
    public class PacketSnapshot
    {
        public PacketSnapshot(IEnumerable<object> values) => this.values = values.ToArray();
        
        public IReadOnlyList<object> Values => values;
        private object[] values;
    }
    
    public class Packet
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        public PacketSnapshot Save() => new PacketSnapshot(map.Values);
        public void Load(PacketSnapshot snapshot)
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
    
    public static class TreeExtensions
    {
        public static bool IsChildOf(this Node node, Node supposedParent)
        {
            var parent = node.Parent;
            if (parent == supposedParent) return true;
            
            while (parent != null)
            {
                parent = parent.Parent;
                if (parent == supposedParent) return true;
            }

            return false;
        }

        public static TNode Append<TNode>(this TNode node, params Node[] nodes) where TNode : Node
        {
            node.Insert(nodes);
            return node;
        }
        public static TNode Mask<TNode>(this TNode node, int input) where TNode : Node
        {
            node.Input = input;
            return node;
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

    public interface ICommand { }

    [Serializable]
    public class PulseCommand : ICommand
    {
        public PulseCommand(Node target) => this.target = target;
        
        public Node Target => target;
        [SerializeField] private Node target;
    }
    
    public class ShutdownCommand : ICommand { }
    
    public class RootNode : Node
    {
        public override bool IsLocked
        {
            get => isLocked || updatedNodes.Any(node => node.IsLocked);
            set => isLocked = value;
        }

        private List<Node> updatedNodes = new List<Node>();
        private Queue<ICommand> commands = new Queue<ICommand>();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Start(Packet packet)
        {
            IsDone = false;
            
            updatedNodes.Clear();
            foreach (var child in Childs)
            {
                if ((output | child.Input) != Output) continue;
                updatedNodes.Add(child);
            }
            
            OnStart(packet);
        }
        protected virtual void OnStart(Packet packet) { }

        public override IEnumerable<Node> Update(Packet packet)
        {
            OnUpdate(packet);
            for (var i = 0; i < updatedNodes.Count; i++)
            {
                i = UpdateNodeAt(i, packet);
                if (i == -1) return null;
            }
            
            if (updatedNodes.Count == 0)
            {
                IsLocked = false;
                Start(packet);
            }

            return null;
        }
        protected virtual void OnUpdate(Packet packet) { }

        public override void Shutdown()
        {
            IsDone = true;
            base.Shutdown();
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void Order(ICommand command) => commands.Enqueue(command);

        private void HandleCommand(ICommand command, Packet packet)
        {
            switch (command)
            {
                case PulseCommand pulse:

                    if (updatedNodes.Contains(pulse.Target))
                    {
                        pulse.Target.Start(packet);
                        break;
                    }
                    
                    if (!pulse.Target.IsChildOf(this)) break;

                    var match = false;
                    for (var i = 0; i < updatedNodes.Count; i++)
                    {
                        if (!updatedNodes[i].IsChildOf(pulse.Target)) continue;

                        match = true;
                        updatedNodes.RemoveAt(i);
                        i--;
                    }

                    if (match)
                    {
                        pulse.Target.Start(packet);
                        updatedNodes.Add(pulse.Target);
                    }
                    break;
                
                case ShutdownCommand shutdown:
                    
                    Debug.Log("Delayed shutdown.");
                    Shutdown();
                    break;
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private int UpdateNodeAt(int index, Packet packet)
        {
            var snapshot = packet.Save();
            var result = updatedNodes[index].Update(packet);
            
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
            updatedNodes.RemoveAt(index);

            var count = result.Count();
            if (count == 0) return index - 1;
            
            updatedNodes.InsertRange(index, result);
            for (var i = 0; i < count; i++) index = UpdateNodeAt(index + i, packet);

            packet.Load(snapshot);
            return index;
        }
    }
    
    public abstract class ProxyNode : Node
    {
        public override void Start(Packet packet)
        {
            IsDone = false;
            OnStart(packet);
        }
        protected virtual void OnStart(Packet packet) { }
        
        public override IEnumerable<Node> Update(Packet packet)
        {
            OnUpdate(packet);
            if (!IsDone) return null;

            var selection = new List<Node>();
            foreach (var child in Childs)
            {
                if ((output | child.Input) != output) continue;
                
                child.Start(packet);
                selection.Add(child);
            }

            return selection;
        }
        protected virtual void OnUpdate(Packet packet) { }

        protected void End(int output)
        {
            IsDone = true;
            this.output = output;
        }
    }

    [Serializable]
    public class Print : ProxyNode
    {
        public Print(string message) => this.message = message;
        
        [SerializeField] private string message;
        
        protected override void OnUpdate(Packet packet)
        {
            Debug.Log(message);
            IsDone = true;
        }
    }

    [Serializable]
    public class Delay : ProxyNode
    {
        public Delay(string tag, float time)
        {
            this.tag = tag;
            this.time = time;
        }

        [SerializeField] private float time;

        private string tag;
        private float timer;

        protected override void OnStart(Packet packet)
        {
            IsLocked = true;
            timer = time;
        }

        protected override void OnUpdate(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //Debug.Log($"[{tag}] : End of timer.");

                IsLocked = false;
                IsDone = true;
            }
        }
    }
}