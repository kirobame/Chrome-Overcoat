using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TiltControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;

        [FoldoutGroup("Pitch"), SerializeField] private Knob pitchKnob;
        [FoldoutGroup("Pitch"), SerializeField] private float groundPitchSmoothing;
        [FoldoutGroup("Pitch"), SerializeField, Range(0.01f, 3.0f)] private float knockback;
        [FoldoutGroup("Pitch"), SerializeField] private float pitchSettling;
        
        [FoldoutGroup("Roll"), SerializeField] private Knob rollKnob;
        
        private float airPitchSmoothing;

        private float pitchAdd;
        private float reduction;

        void Awake()
        {
            Events.Subscribe<float>(PlayerEvent.OnFire, OnFire);
            airPitchSmoothing = pitchKnob.Smoothing;
        }
        void OnDestroy() => Events.Unsubscribe<float>(PlayerEvent.OnFire, OnFire);

        void Update()
        {
            var delta = body.transform.InverseTransformVector(body.Delta);

            pitchKnob.Smoothing = body.IsGrounded ? groundPitchSmoothing : airPitchSmoothing;
            var pitch = pitchKnob.Process(delta.y + pitchAdd);
            var roll = rollKnob.Process(delta.x);
            
            transform.localEulerAngles = new Vector3(pitch, 0.0f, roll);
            pitchAdd = Mathf.SmoothDamp(pitchAdd, 0.0f, ref reduction, pitchSettling);
        }

        void OnFire(float force) => pitchAdd = force * -knockback;
    }
}