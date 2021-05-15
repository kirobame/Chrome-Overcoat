using System.Linq;
using UnityEngine;

namespace Chrome
{
    public class Blackboard : IBlackboard
    {
        public static IBlackboard Global
        {
            get
            {
                if (global == null) global = new Blackboard();
                return global;
            }
        }
        private static IBlackboard global;

        //--------------------------------------------------------------------------------------------------------------/

        public Blackboard() => root = new BlackboardEntry("", "root", new NullRegistry());
        
        private BlackboardEntry root;
        
        public BlackboardSnapshot Save() => new BlackboardSnapshot(root.Copy());
        public void Load(BlackboardSnapshot snapshot) => root.Inject(snapshot.CopiedRoot);
  
        //--------------------------------------------------------------------------------------------------------------/
        
        public void Remove(string path)
        {
            if (path.Count(letter => letter == '.') > 0)
            {
                var splitIndex = path.LastIndexOf('.');
                var parentPath = path.Substring(0, splitIndex);
                var childName = path.Substring(splitIndex + 1, path.Length - parentPath.Length - 1);
                
                if (root.TryGetEntryAt(path, out var entry)) entry.Remove(childName);
            }
            else root.Remove(path);
        }
        
        public void SetRaw(string path, object value)
        {
            var entry = root.GetEntryAt(path);
            entry.Registry.Set(value);
        }
        public void Set<T>(string path, T value)
        {
            var entry = root.GetEntryAt(path);
            entry.Registry = new WrapperRegistry<T>(value);
        }
        public void SetRegistry(string path, IRegistry registry)
        {
            var entry = root.GetEntryAt(path);
            entry.Registry = registry;
        }

        public bool TryGetAny<T>(out IRegistry<T> registry)
        {
            if (root.TryGetRegistry<T>(out var output))
            {
                registry = output;
                return true;
            }
            else
            {
                registry = default;
                return false;
            }
        }
        
        public T Get<T>(string path)
        {
            TryGet<T>(path, out var value);
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
        
        public TRegistry GetRegistry<TRegistry>(string path)  where TRegistry : IRegistry
        {
            TryGetRegistry<TRegistry>(path, out var value);
            return value;
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
}