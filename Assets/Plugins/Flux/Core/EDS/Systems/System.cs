namespace Flux.EDS
{
    public abstract class System
    {
        public System() { }

        public bool IsActive { get; protected set; } = true;
        
        public virtual void Initialize() { }
        public virtual void Shutdown() { }
        
        public virtual void Update() { }
    }
}