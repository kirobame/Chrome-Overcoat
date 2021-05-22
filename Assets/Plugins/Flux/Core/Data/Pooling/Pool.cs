using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flux.Data
{
    public abstract class Pool : MonoBehaviour { }
    
    public abstract class Pool<T> : Pool
    {
        public abstract void Stock(Poolable<T> poolable);
    }
    
    public abstract class Pool<T,TPoolable> : Pool<T> where TPoolable : Poolable<T>
    {
        public event Action onReady;
        
        public bool IsOperational { get; private set; }
        public IEnumerable<TPoolable> UsedInstances => usedInstances;

        protected abstract IList<Provider<T,TPoolable>> Providers { get; }

        private Dictionary<TPoolable, Queue<TPoolable>> availableInstances = new Dictionary<TPoolable, Queue<TPoolable>>();
        private HashSet<TPoolable> usedInstances = new HashSet<TPoolable>();
        
        private byte readiness;
        
        void Awake()
        {
            IsOperational = false;

            readiness = (byte)Providers.Count;
            foreach (var provider in Providers) PrepareProvider(provider);
        }

        private void PrepareProvider(Provider<T, TPoolable> provider)
        {
            provider.onLoaded += OnProviderReady;
            provider.Bootup();
        }
        void OnProviderReady(Provider<T,TPoolable> source)
        {
            source.onLoaded -= OnProviderReady;

            readiness--;
            if (readiness == 0)
            {
                foreach (var provider in Providers)
                {
                    var queue = new Queue<TPoolable>(provider.Instances);
                    availableInstances.Add(provider.Prefab, queue);
                }
                
                IsOperational = true;
                
                onReady?.Invoke();
                OnReady();
            }
        }
        protected virtual void OnReady() { }
        
        public virtual void AddProvider(Provider<T, TPoolable> provider) { }

        public T RequestSingle() => RequestSinglePoolable().Value;
        public T RequestSingle(TPoolable key) => RequestSinglePoolable(key).Value;
        
        public TPoolable RequestSinglePoolable() => RequestPoolable(Providers[0].Prefab, 1).First();
        public TPoolable RequestSinglePoolable(TPoolable key) => RequestPoolable(key, 1).First();

        public T[] Request(int count) => RequestPoolable(count).Select(poolable => poolable.Value).ToArray();
        public T[] Request(TPoolable key, int count) => RequestPoolable(key, count).Select(poolable => poolable.Value).ToArray();
        
        public TPoolable[] RequestPoolable(int count) => RequestPoolable(Providers[0].Prefab, count);
        public TPoolable[] RequestPoolable(TPoolable key, int count)
        {
            if (!IsOperational)
            {
                throw new InvalidOperationException($"The Pool : {this} is not operational !");
                return null;
            }
            
            var request = new TPoolable[count];
            if (availableInstances.TryGetValue(key, out var queue))
            {
                int index;
                for (index = 0; index < count - queue.Count; index++)
                {
                    var instance = Instantiate(key, transform);
                    Claim(instance, key);
                    
                    request[index] = instance;
                    index++;
                }
                
                for (var i = index; i < count; i++)
                {
                    var instance = queue.Dequeue();
                
                    Claim(instance, key);
                    request[i] = instance;
                }
            }
            else
            {
                availableInstances.Add(key, new Queue<TPoolable>());
                for (var i = 0; i < count; i++)
                {
                    var instance = Instantiate(key);
                    Claim(instance, key);
                    
                    request[i] = instance;
                }
            }

            return request;
        }

        public override void Stock(Poolable<T> poolable) => Stock(poolable as TPoolable);
        public void Stock(TPoolable poolable)
        {
            if (this == null || !gameObject.activeInHierarchy) return;

            poolable.Reboot();
            poolable.gameObject.SetActive(false);
            
            StartCoroutine(ParentingRoutine(poolable));
        }
        private IEnumerator ParentingRoutine(TPoolable poolable)
        {
            yield return new WaitForEndOfFrame();

            availableInstances[(TPoolable)poolable.Key].Enqueue(poolable);
            usedInstances.Remove(poolable);
            
            poolable.transform.SetParent(transform, false);
        }

        private void Claim(TPoolable poolable, TPoolable key)
        {
            poolable.SetOrigin(this, key);
            poolable.Prepare();
            
            poolable.gameObject.SetActive(true);
            poolable.transform.SetParent(null, false);
            
            usedInstances.Add(poolable);
        }
    }
}