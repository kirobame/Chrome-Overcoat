using UnityEngine;

namespace Chrome
{
    public class IdleIntensityAffect : IntensityAffect
    {
        [SerializeField] private float affect;
        [SerializeField] private float sampling;
        [SerializeField] private int precision;
        [SerializeField] private float threshold;
        [SerializeField, Range(0.0f, 1.0f)] private float percentage;
        
        private Cadence cadence;
        private Vector3[] history;
        
        private Vector3 aggregation;
        private int count;

        void Awake()
        {
            aggregation = Vector3.zero;
            count = 0;
            
            history = new Vector3[Mathf.RoundToInt(precision * sampling) - 1];
            for (int i = 0; i < history.Length; i++)
            {
                var point = transform.position + Vector3.back * (threshold / sampling * i);
                
                history[i] = point;
                aggregation += point;
            }
            
            cadence = new Cadence(1.0f / precision);
        }

        void Update()
        {
            if (!cadence.Update(Time.deltaTime))
            {
                count++;
                aggregation += transform.position;
                var position = aggregation / count;

                var center = position;
                for (int i = 0; i < history.Length; i++) center += history[i];
                center /= history.Length + 1;

                var successes = 0;
                if (Vector3.Distance(center, position) <= threshold) successes++;

                for (int i = 0; i < history.Length; i++)
                {
                    if (Vector3.Distance(center, history[i]) > threshold) continue;
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

            aggregation = Vector3.zero;
            count = 0;
        }
    }
}