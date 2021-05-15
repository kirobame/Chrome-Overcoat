using System;
using System.Collections.Generic;

namespace Chrome
{
    public class BlackboardEntry : IEquatable<BlackboardEntry>
    {
        public BlackboardEntry(string parent, string name, IRegistry registry)
        {
            Registry = registry;
                
            Parent = parent;
            Name = name;
             
            children = new HashSet<BlackboardEntry>();
        }

        public IRegistry Registry { get; set; }
            
        public string Parent { get; private set; }
        public string Name { get; private set; }

        public IEnumerable<BlackboardEntry> Children => children;
        private HashSet<BlackboardEntry> children;

        //--------------------------------------------------------------------------------------------------------------/

        public void Remove(string childName) => children.RemoveWhere(child => child.Name == childName);
        public void Add(BlackboardEntry child) => children.Add(child);
            
        public bool TryGetRegistry<T>(out IRegistry<T> registry)
        {
            if (Registry is IRegistry<T> match)
            {
                registry = match;
                return true;
            }
            else
            {
                foreach (var child in children)
                {
                    if (child.TryGetRegistry<T>(out registry)) return true;
                }
            }

            registry = default;
            return false;
        }
            
        public BlackboardEntry GetEntryAt(string path) => GetEntryAt(path.Split('.'), 0);
        private BlackboardEntry GetEntryAt(string[] path, int advancement)
        {
            foreach (var child in children)
            {
                if (child.Name != path[advancement]) continue;

                if (advancement == path.Length - 1) return child;
                else return child.GetEntryAt(path, advancement + 1);
            }

            var relay = new BlackboardEntry(Name, path[advancement], new NullRegistry());
            children.Add(relay);

            if (advancement == path.Length - 1) return relay;
            else return relay.GetEntryAt(path, advancement + 1);
        }

        public bool TryGetEntryAt(string path, out BlackboardEntry entry) => TryGetEntryAt(path.Split('.'), 0, out entry);            
        private bool TryGetEntryAt(string[] path, int advancement, out BlackboardEntry entry)
        {
            foreach (var child in children)
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

        //--------------------------------------------------------------------------------------------------------------/

        public void Inject(BlackboardEntry entry)
        {
            Registry = entry.Registry;
                
            children.IntersectWith(entry.Children);

            using var selfEnumerator = children.GetEnumerator();
            using var otherEnumerator = entry.children.GetEnumerator();

            selfEnumerator.Current.Inject(otherEnumerator.Current);
            while (selfEnumerator.MoveNext())
            {
                otherEnumerator.MoveNext();
                selfEnumerator.Current.Inject(otherEnumerator.Current);
            }
        }
        public BlackboardEntry Copy()
        {
            var copy = new BlackboardEntry(Parent, Name, Registry.Copy());
            foreach (var child in children) copy.Add(child.Copy());

            return copy;
        }

        public override bool Equals(object obj) => Equals((BlackboardEntry) obj);
        public bool Equals(BlackboardEntry other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();
    }
}