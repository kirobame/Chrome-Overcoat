using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class TiltControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;

        [FoldoutGroup("Pitch"), SerializeField] private Knob pitchKnob;
        [FoldoutGroup("Pitch"), SerializeField] private float groundPitchSmoothing;
        
        [FoldoutGroup("Roll"), SerializeField] private Knob rollKnob;
        
        private float airPitchSmoothing;

        void Awake() => airPitchSmoothing = pitchKnob.Smoothing;
        
        void Update()
        {
            var delta = body.transform.InverseTransformVector(body.Controller.velocity);

            pitchKnob.Smoothing = body.IsGrounded ? groundPitchSmoothing : airPitchSmoothing;
            var pitch = pitchKnob.Process(delta.y);
            var roll = rollKnob.Process(delta.x);
            
            transform.localEulerAngles = new Vector3(pitch, 0.0f, roll);
        }
    }
}