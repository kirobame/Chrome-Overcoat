using UnityEngine;

namespace Chrome
{
    public class FlowIntensityAffect : IntensityAffect
    {
        protected override void PrepareInjection() => body = injections.Register(new AnyValue<CharacterBody>());

        //--------------------------------------------------------------------------------------------------------------/
        
        [SerializeField] private float affect;
        [SerializeField] private float sampling;
        [SerializeField] private int precision;
        [SerializeField] private float threshold;
        [SerializeField, Range(0.0f, 1.0f)] private float percentage;

        private Cadence cadence;
        private float[] history;
        
        private float aggregation;
        private int count;

        private IValue<CharacterBody> body;

        void Awake()
        {
            aggregation = 0.0f;
            count = 0;
            
            history = new float[Mathf.RoundToInt(precision * sampling) - 1];
            for (var i = 0; i < history.Length; i++) history[i] = 0.0f;
            
            cadence = new Cadence(1.0f / precision);
        }

        void Update()
        {
            if (!cadence.Update(Time.deltaTime))
            {
                count++;
                aggregation += body.Value.Delta.magnitude;
                var speed = aggregation / count;

                var successes = 0;
                if (speed >= threshold) successes++;

                for (var i = 0; i < history.Length; i++)
                {
                    if (history[i] < threshold) continue;
                    successes++;
                }

                var result = successes / (float)(history.Length + 1);
                if (result >= percentage) Intensity.affect += affect * Time.deltaTime;
                
                return;
            }

            var next = history[0];
            for (var i = 0; i < history.Length - 1; i++)
            {
                var cache = next;
                next = history[i + 1];
                
                history[i + 1] = cache;
            }
            
            history[0] = aggregation / count;
            aggregation += history[0];
            
            aggregation = 0.0f;
            count = 0;
        }
    }
}