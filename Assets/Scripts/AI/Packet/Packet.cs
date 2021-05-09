using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class Packet
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        public PacketSnapshot Save() => new PacketSnapshot(map);
        public void Load(PacketSnapshot snapshot)
        {
            var keys = new HashSet<Type>(map.Keys);
            foreach (var kvp in snapshot.Values)
            {
                if (!keys.Contains(kvp.Key))
                {
                    map.Add(kvp.Key, kvp.Value);
                    continue;
                }

                map[kvp.Key] = kvp.Value;
                keys.Remove(kvp.Key);
            }

            foreach (var key in keys) map.Remove(key);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        public T Get<T>() => (T)map[typeof(T)];
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

        public bool TryGetThoroughly<T>(out KeyValuePair<Type,T> kvp)
        {
            foreach (var candidate in map)
            {
                if (!(candidate.Value is T cast)) continue;

                kvp = new KeyValuePair<Type, T>(candidate.Key, cast);
                return true;
            }

            kvp = default;
            return false;
        }
        
        public void Set<T>(T value)
        {
            if (map.ContainsKey(typeof(T))) map[typeof(T)] = value;
            else map.Add(typeof(T), value);
        }
        public void Set(Type type, object value)
        {
            if (map.ContainsKey(type)) map[type] = value;
            else map.Add(type, value);
        }
    }
}