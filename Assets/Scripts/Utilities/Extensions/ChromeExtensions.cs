﻿using System;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using Flux.EDS;
using UnityEngine;

namespace Chrome
{
    public static class ChromeExtensions
    {
        private static SpawnLocations[] allLocations;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootup() => allLocations = Enum.GetValues(typeof(SpawnLocations)).Cast<SpawnLocations>().ToArray();

        //--------------------------------------------------------------------------------------------------------------/
    
        public static IEnumerable<SpawnLocations> Split(this SpawnLocations locations)
        {
            var output = new List<SpawnLocations>();
            foreach(var location in allLocations)
            {
                if ((locations & location) != 0) output.Add(location);
            }

            return output;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public static IValue<T> Register<T>(this List<IValue> injections, IValue<T> value)
        {
            injections.Add(value);
            return value;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public static TOwner Recurse<TOwner>(this IAssignable assignee, int count = 1)
        {
            if (count <= 1) return (TOwner)assignee.Value;
            else
            {
                var current = (IAssignable)assignee.Value;
                for (var i = 0; i < count - 1; i++) current = (IAssignable)assignee.Value;
                
                return (TOwner)current.Value;
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public static T[] Fetch<T,TFilter>(this Transform source)
        {
            var values = new List<T>();
            values.AddRange(source.GetComponents<T>());
            
            for (var i = 0; i < source.childCount; i++)
            {
                var child = source.GetChild(i);
                FetchFrom<T,TFilter>(child, values);
            }

            return values.ToArray();
        }
        private static void FetchFrom<T,TFilter>(Transform transform, List<T> values)
        {
            if (transform.HasComponent<TFilter>()) return;
            
            values.AddRange(transform.GetComponents<T>());
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                FetchFrom<T,TFilter>(child, values);
            }
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public static IValue<T> Cache<T>(this T value) => new CachedValue<T>(value);
        public static IValue<T> Reference<T>(this string path, ReferenceType type = ReferenceType.Local)
        {
            switch (type)
            {
                case ReferenceType.Global:
                    return new GloballyReferencedValue<T>(path);
                
                case ReferenceType.SubGlobal:
                    return new SubGloballyReferencedValue<T>(path);
                
                case ReferenceType.Local:
                    return new LocallyReferencedValue<T>(path);
                
                case ReferenceType.Nested:
                    return new NestedReferencedValue<T>(path);
            }

            return null;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static ComputeAimDirection CreateComputeAimDirection()
        {
            var mask = LayerMask.GetMask("Environment", "Entity");
            var pivot = Refs.PIVOT.Reference<Transform>();
            var fireAnchor = Refs.FIREANCHOR.Reference<Transform>();
            var collider = Refs.COLLIDER.Reference<Collider>();
            
            return new ComputeAimDirection("shootDir", mask, fireAnchor, pivot, collider);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static void RelayDamage(this InteractionHub hub, IIdentity source, float amount)
        {
            hub.Relay<IDamageable>((damageable, depth) =>
            {
                if (damageable.Identity.Faction == source.Faction) return false;

                damageable.Hit(source, amount, source.Packet);
                return true;
            });
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static void AddElement<TElement>(this Packet packet, TElement element)
        {
            if (packet.TryGet<List<TElement>>(out var list)) list.Add(element);
            else
            {
                list = new List<TElement>() { element };
                packet.Set(list);
            }
        }
        
        public static void SetKeyValuePair<TKey, TValue>(this Packet packet, TKey key, TValue value)
        {
            if (packet.TryGet<Dictionary<TKey, TValue>>(out var dictionary)) dictionary.Add(key, value);
            else
            {
                dictionary = new Dictionary<TKey, TValue>();
                dictionary.Add(key, value);
                
                packet.Set(dictionary);
            }
        }
        public static bool TryGetValueAt<TKey, TValue>(this Packet packet, TKey key, out TValue value)
        {
            if (packet.TryGet<Dictionary<TKey, TValue>>(out var dictionary) && dictionary.TryGetValue(key, out value)) return true;
            else
            {
                value = default;
                return false;
            }
        }
        public static TValue GetValueAt<TKey, TValue>(this Packet packet, TKey key)
        {
            var dictionary = packet.Get<Dictionary<TKey, TValue>>();
            return dictionary[key];
        }
        public static TValue GetOrCreateValueAt<TKey, TValue>(this Packet packet, TKey key) where TValue : new()
        {
            if (!packet.TryGet<Dictionary<TKey, TValue>>(out var dictionary))
            {
                var value = new TValue();
                
                dictionary = new Dictionary<TKey, TValue>();
                dictionary.Add(key, value);
                
                packet.Set(dictionary);

                return value;
            }

            if (dictionary.TryGetValue(key, out var fetchedValue)) return fetchedValue;
            else
            {
                var value = new TValue();
                dictionary.Add(key, value);

                return value;
            }
        }

        //--------------------------------------------------------------------------------------------------------------/

        public static void RebootLocally(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}