using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Flux.Data
{
    public abstract class Poolable : MonoBehaviour { }
    
    public abstract class Poolable<T> : Poolable, IInjectable<T>
    {
        public Poolable<T> Key => key;
        
        public T Value => value;
        [SerializeField] private T value;
        
        private Poolable<T> key;
        protected Pool<T> origin;
        
        protected virtual void OnDisable()
        {
            if (origin == null) return;
            origin.Stock(this);
        }

        public void SetOrigin(Pool<T> origin, Poolable<T> key)
        {
            this.key = key;
            this.origin = origin;
        }
        
        public virtual void Prepare() { }
        public virtual void Reboot() { }

        void IInjectable<T>.Inject(T value) => this.value = value;
    }
}