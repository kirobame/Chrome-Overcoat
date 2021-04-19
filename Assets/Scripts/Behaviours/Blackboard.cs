using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chrome
{
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

            public void Remove(string childName) => childs.RemoveAll(child => child.Name == childName);

            public Entry GetEntryAt(string path) => GetEntryAt(path.Split('.'), 0);
            private Entry GetEntryAt(string[] path, int advancement)
            {
                foreach (var child in childs)
                {
                    if (child.Name != path[advancement]) continue;

                    if (advancement == path.Length - 1) return child;
                    else return child.GetEntryAt(path, advancement + 1);
                }

                var relay = new Entry(Name, path[advancement], new NullRegistry());
                childs.Add(relay);

                if (advancement == path.Length - 1) return relay;
                else return relay.GetEntryAt(path, advancement + 1);
            }

            public bool TryGetEntryAt(string path, out Entry entry) => TryGetEntryAt(path.Split('.'), 0, out entry);            
            private bool TryGetEntryAt(string[] path, int advancement, out Entry entry)
            {
                foreach (var child in childs)
                {
                    if (child.Name != path[advancement]) continue;

                    if (advancement == path.Length - 1)
                    {
                        entry = child;
                        return true;
                    }
                    else
                    {
                        var success = child.TryGetEntryAt(path, advancement + 1, out entry);
                        return success;
                    }
                }

                entry = null;
                return false;
            }
        }

        #endregion

        public Blackboard() => root = new Entry("", "root", new NullRegistry());
        
        private Entry root;

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
}