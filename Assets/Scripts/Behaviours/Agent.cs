using System.Collections.Generic;
using System.Linq;
using Flux;
using Sirenix.OdinInspector;
using UnityEngine;

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
    
    public class LineOfSight : MonoBehaviour
    {
        [SerializeField] private LayerMask blockingMask;
        
        public bool CanSee(Vector3 point)
        {
            var direction = point - transform.position;
            var ray = new Ray(transform.position, direction);

            return Physics.Raycast(ray, direction.magnitude, blockingMask);
        }
        public bool CanSee(Collider collider)
        {
            var corners = collider.bounds.GetCorners();
            return corners.Any(CanSee);
        }
    }

    public class UnityRegistry : MonoBehaviour, IRegistry
    {
        public object RawValue => value;
        [SerializeField] private Object value;
        [SerializeField] private new string name;
        
        public bool IsSource => isSource;
        [Space, SerializeField] private bool isSource;

        [SerializeField, ShowIf("isSource")] private bool isGlobal;

        void Awake()
        {
            if (!isGlobal) return;
            Collect(Blackboard.Global, string.Empty);
        }

        public void Collect(Blackboard board, string path)
        {
            if (path != string.Empty) path += $".{name}";
            else path = name;
            
            board.Set(value, path);

            if (isSource)
            {
                foreach (var registry in GetComponents<UnityRegistry>())
                {
                    if (registry == this) continue;
                    registry.Collect(board, path);
                }
            }
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                
                if (!child.TryGetComponent<UnityRegistry>(out var registry) || registry.IsSource) continue;
                registry.Collect(board, path);
            }
        }
    }
    
    public class Agent : MonoBehaviour
    {
        [SerializeField] private PhysicBody body;
        [SerializeField] private LineOfSight lineOfSight;
        
        private RootNode behaviourTree;
        private Packet packet;

        void Awake()
        {
            packet = new Packet();
            packet.Set(body);
            packet.Set(lineOfSight);
            
            
        }
    }

    public abstract class ConditionalNode : ProxyNode
    {
        protected override void OnUpdate(Packet packet)
        {
            if (Check(packet)) output = 1;
            else output = 0;

            IsDone = true;
        }
        protected abstract bool Check(Packet packet);
    }

    public abstract class CanSeePlayer : ConditionalNode
    {
        protected override bool Check(Packet packet)
        {
            if (!packet.TryGet<LineOfSight>(out var lineOfSight)) return false;

            var player = Blackboard.Global.Get<PhysicBody>("player.body");
            return lineOfSight.CanSee(player.Controller);
        }
    }
}