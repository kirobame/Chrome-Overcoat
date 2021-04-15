using System.Collections.Generic;
using Flux;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome
{
    public interface IRegistry
    {
        object RawValue { get; }
    }
    public interface IRegistry<out T> : IRegistry
    {
        T Value { get; }
    }

    public class NullRegistry : IRegistry
    {
        public object RawValue => null;
    }
    
    public class WrapperRegistry<T> : IRegistry<T>
    {
        public WrapperRegistry(T value) => this.value = value;
        
        public object RawValue => value;
        
        public T Value => value;
        private T value;
    }
    
    public class Blackboard
    {
        public static Blackboard Global
        {
            get
            {
                if (global == null) global = new Blackboard();
                return global;
            }
        }
        private static Blackboard global;
        
        #region Nested Types

        private class Entry
        {
            public Entry(string parent, string name, IRegistry registry)
            {
                Registry = registry;
                
                Parent = parent;
                Name = name;
             
                childs = new List<Entry>();
            }

            public IRegistry Registry { get; set; }
            
            public string Parent { get; private set; }
            public string Name { get; private set; }

            public IReadOnlyList<Entry> Childs => childs;
            private List<Entry> childs;

            public Entry GetEntryAt(string path) => GetEntryAt(path.Split('.'), 0);
            private Entry GetEntryAt(string[] path, int advancement)
            {
                if (advancement == path.Length - 1) return this;
                
                foreach (var child in childs)
                {
                    if (child.Name != path[advancement]) continue;
                    return child.GetEntryAt(path, advancement + 1);
                }

                var relay = new Entry(Name, path[advancement], new NullRegistry());
                childs.Add(relay);

                return relay.GetEntryAt(path, advancement + 1);
            }

            public bool TryGetEntryAt(string path, out Entry entry) => TryGetEntryAt(path.Split('.'), 0, out entry);            
            private bool TryGetEntryAt(string[] path, int advancement, out Entry entry)
            {
                if (advancement == path.Length - 1)
                {
                    entry = this;
                    return true;
                }
                
                foreach (var child in childs)
                {
                    if (child.Name != path[advancement]) continue;
                    
                    var success = child.TryGetEntryAt(path, advancement + 1, out entry);
                    return success;
                }

                entry = null;
                return false;
            }
        }

        #endregion

        public Blackboard() => root = new Entry("", "Root", new NullRegistry());
        
        private Entry root;
        
        public void Set<T>(T value, string path)
        {
            var entry = root.GetEntryAt(path);
            entry.Registry = new WrapperRegistry<T>(value);
        }
        public void SetRegistry(IRegistry registry, string path)
        {
            var entry = root.GetEntryAt(path);
            entry.Registry = registry;
        }

        public T Get<T>(string path)
        {
            TryGet<T>(path, out var value);
            return value;
        }
        public TRegistry GetRegistry<TRegistry>(string path)  where TRegistry : IRegistry
        {
            TryGetRegistry<TRegistry>(path, out var value);
            return value;
        }
        
        public bool TryGet<T>(string path, out T value)
        {
            if (root.TryGetEntryAt(path, out var entry))
            {
                if (entry.Registry is IRegistry<T> registry)
                {
                    value = registry.Value;
                    return true;
                }
                else if (entry.Registry.RawValue is T castedValue)
                {
                    value = castedValue;
                    return true;
                }
            }

            value = default;
            return false;
        }
        public bool TryGetRegistry<TRegistry>(string path, out TRegistry registry) where TRegistry : IRegistry
        {
            if (root.TryGetEntryAt(path, out var entry))
            {
                if (entry.Registry is TRegistry castedRegistry)
                {
                    registry = castedRegistry;
                    return true;
                }
            }

            registry = default;
            return false;
        }
    }

    public class Agent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMesh;
        [SerializeField] private LineOfSight lineOfSight;
        [SerializeField] private Transform aim;
        [SerializeField] private Transform fireAnchor;
        
        private RootNode behaviourTree;
        private Blackboard board;
        private Packet packet;

        void Awake()
        {
            board = new Blackboard();
            board.Set(aim, "aim");
            board.Set(fireAnchor, "aim.fireAnchor");
            
            packet = new Packet();
            packet.Set(navMesh);
            packet.Set(lineOfSight);
            packet.Set(board);

            behaviourTree = new RootNode();
            var conditionalNode = new CanSeePlayer();
            
            behaviourTree.Append(
                conditionalNode.Append(
                    new StopMoving().Mask(0b_0001).Append(
                        new Delay("TRUE", 1.0f).Append(
                            new Pulse(behaviourTree, conditionalNode)),
                        new LookAt()),
                    new MoveTo().Mask(0b_0010).Append(
                        new Delay("FALSE", 1.0f))));
        }

        void Start() => behaviourTree.Start(packet);
        void Update() => behaviourTree.Update(packet);
    }

    public abstract class ConditionalNode : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            if (Check(packet)) output = 0b_0001;
            else output = 0b_0010;

            IsDone = true;
        }
        protected abstract bool Check(Packet packet);
    }

    public class CanSeePlayer : ConditionalNode
    {
        protected override bool Check(Packet packet)
        {
            if (!packet.TryGet<LineOfSight>(out var lineOfSight)) return false;

            var player = Blackboard.Global.Get<PhysicBody>("player.body");
            return lineOfSight.CanSee(player.Controller);
        }
    }

    public class StopMoving : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            if (packet.TryGet<NavMeshAgent>(out var navMesh)) navMesh.isStopped = true;
            IsDone = true;
        }
    }
    
    public class MoveTo : ProxyNode
    {
        protected override void OnStart(Packet packet)
        {
            if (packet.TryGet<NavMeshAgent>(out var navMesh)) navMesh.isStopped = false;
        }

        protected override void OnUpdate(Packet packet)
        {
            if (packet.TryGet<NavMeshAgent>(out var navMesh))
            {
                var player = Blackboard.Global.Get<Transform>("player");
                navMesh.SetDestination(player.position);
            }
            
            IsDone = true;
        }
    }

    // Should not be looping indefinitely
    // Could be done via another node which runs childs indefinitely 
    // --> Try implementation with a simple root node. Should naturally loop as soon as the LookAt logic is done
    public class LookAt : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<Blackboard>();
            if (!board.TryGet<Transform>("aim", out var aim))
            {
                IsDone = true;
                return;
            }
            
            var player = Blackboard.Global.Get<Transform>("player");
            aim.LookAt(player.transform.position);
        }
    }

    public class Pulse : ProxyNode
    {
        public Pulse(RootNode root, Node target)
        {
            this.root = root;
            this.target = target;
        }
        
        private RootNode root;
        private Node target;

        protected override void OnUpdate(Packet packet)
        {
            IsDone = true;
            root.Order(new PulseCommand(target));
        }
    }
    
    // Implement logic as a Root node
    // Add behaviour dictating whether a node is a breakpoint or not
    // --> Defines if control can be passed to children of the node or not
    public class ClickInput : ProxyNode
    {
        public ClickInput(float duration) => this.duration = duration;
        
        private float duration;
        private float timer;

        protected override void OnStart(Packet packet)
        {
            timer = duration;
            output = 0b_0001;
            
            this.StartChilds(packet);
        }

        protected override void OnUpdate(Packet packet)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                output = 0b_0010;
                this.UpdateChilds(packet);
            }
            else this.UpdateChilds(packet);
        }
    }

    public class TimeFilter : ProxyNode
    {
        
    }

    public class ShootAt : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            base.OnUpdate(packet);
        }
    }
}