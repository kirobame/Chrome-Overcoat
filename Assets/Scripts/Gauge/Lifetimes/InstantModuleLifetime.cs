namespace Chrome
{
    public struct InstantModuleLifetime : IGaugeModuleLifetime
    {
        public bool Update(Gauge source) => true;
    }
}