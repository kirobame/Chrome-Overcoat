using UnityEngine;

namespace Chrome
{
    public class SpeedIntensityAffect : IntensityAffect
    {
        protected override void PrepareInjection() => body = injections.Register(new AnyValue<CharacterBody>());

        //--------------------------------------------------------------------------------------------------------------/
        
        [SerializeField] private AnimationCurve map;
        [SerializeField] private float smoothing;

        private float damping;
        private float current;
        
        private IValue<CharacterBody> body;

        void Update()
        {
            var goal = map.Evaluate(body.Value.Delta.magnitude);
            current = Mathf.SmoothDamp(current, goal, ref damping, smoothing);

            Intensity.affect += current * Time.deltaTime;
        }
    }
}