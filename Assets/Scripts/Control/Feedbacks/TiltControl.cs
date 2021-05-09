using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TiltControl : MonoBehaviour, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Pitch"), SerializeField] private Knob pitchKnob;
        [FoldoutGroup("Pitch"), SerializeField] private float groundPitchSmoothing;
        [FoldoutGroup("Pitch"), SerializeField, Range(0.01f, 3.0f)] private float knockback;
        [FoldoutGroup("Pitch"), SerializeField] private float pitchSettling;
        
        [FoldoutGroup("Roll"), SerializeField] private Knob rollKnob;

        private IValue<CharacterBody> body;
        
        private float airPitchSmoothing;

        private float pitchAdd;
        private float reduction;

        void Awake()
        {
            body = new AnyValue<CharacterBody>();
            injections = new IValue[] { body };
            
            Events.Subscribe<float>(PlayerEvent.OnFire, OnFire);
            airPitchSmoothing = pitchKnob.Smoothing;
        }
        void OnDestroy() => Events.Unsubscribe<float>(PlayerEvent.OnFire, OnFire);

        void Update()
        {
            var delta = body.Value.transform.InverseTransformVector(body.Value.Delta);

            pitchKnob.Smoothing = body.Value.IsGrounded ? groundPitchSmoothing : airPitchSmoothing;
            var pitch = ComputePitch(delta);
            var yaw = ComputeYaw(delta);
            var roll = ComputeRoll(delta);
            
            transform.localEulerAngles = new Vector3(pitch, yaw, roll);
            pitchAdd = Mathf.SmoothDamp(pitchAdd, 0.0f, ref reduction, pitchSettling);
        }

        protected virtual float ComputePitch(Vector3 bodyDelta)
        {
            pitchKnob.Smoothing = body.Value.IsGrounded ? groundPitchSmoothing : airPitchSmoothing;
            return pitchKnob.Process(bodyDelta.y + pitchAdd);
        }
        protected virtual float ComputeYaw(Vector3 bodyDelta) => 0.0f;
        protected virtual float ComputeRoll(Vector3 bodyDelta) => rollKnob.Process(bodyDelta.x);

        void OnFire(float force) => pitchAdd = force * -knockback;
    }
}