namespace Chrome
{
    public class GaugeDivisionModule : GaugeModule, IGaugeImpact
    {
        public GaugeDivisionModule(float value) => this.value = value;
        
        private float value;

        public float ComputeImpact(float value) => value / this.value;
    }
}