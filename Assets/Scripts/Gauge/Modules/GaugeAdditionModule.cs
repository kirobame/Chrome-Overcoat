namespace Chrome
{
    public class GaugeAdditionModule : GaugeModule, IGaugeImpact
    {
        public GaugeAdditionModule(float value) => this.value = value;
        
        private float value;

        public float ComputeImpact(float value) => value + this.value;
    }
}