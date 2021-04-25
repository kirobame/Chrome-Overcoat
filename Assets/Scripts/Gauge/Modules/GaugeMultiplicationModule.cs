namespace Chrome
{
    public class GaugeMultiplicationModule : GaugeModule, IGaugeImpact
    {
        public GaugeMultiplicationModule(float value) => this.value = value;
        
        private float value;

        public float ComputeImpact(float value) => value * this.value;
    }
}