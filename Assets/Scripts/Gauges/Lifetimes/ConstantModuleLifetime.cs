namespace Chrome
{
    public struct ConstantModuleLifetime : IGaugeModuleLifetime
    {
        public bool Update(Gauge source) => false;
    }
}