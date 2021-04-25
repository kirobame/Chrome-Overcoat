namespace Chrome
{
    public abstract class GaugeModule
    {
        public Gauge Source { get; protected set; }
        public bool IsDone { get; protected set; } = false;

        public IGaugeModuleLifetime lifetime = new InstantModuleLifetime();     
        
        public virtual void Initialize(Gauge source) => Source = source;
        public virtual void Update() => IsDone = lifetime.Update(Source);
    }
}