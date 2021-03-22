namespace Flux.EDS
{
    public abstract class System : IBootable
    {
        public System() { }

        public bool IsActive { get; protected set; } = true;
        
        public virtual void Bootup() { }
        public virtual void Shutdown() { }
        
        public virtual void Update() { }
    }
}