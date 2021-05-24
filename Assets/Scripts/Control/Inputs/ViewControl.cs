using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class ViewControl : InputControl<ViewControl>
    {
        protected override void SetupInputs()
        {
            inputs = new CachedValue<Vector2>(Vector2.zero);
            input.Value.BindValue<Vector2>(InputRefs.VIEW, this, inputs);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public Vector2 Inputs => inputs.Value;
        public Vector2 Delta => new Vector2(yawKnob.Value, pitchKnob.Value);
        
        [FoldoutGroup("Yaw"), SerializeField] private Transform yawTarget;
        [FoldoutGroup("Yaw"), SerializeField] private Knob yawKnob;
        [FoldoutGroup("Yaw"), SerializeField] private Acceleration yawAcceleration;
     
        [FoldoutGroup("Pitch"), SerializeField] private Transform pitchTarget;
        [FoldoutGroup("Pitch"), SerializeField] private Knob pitchKnob;
        [FoldoutGroup("Pitch"), SerializeField] private Vector2 pitchRange;

        private CachedValue<Vector2> inputs;
        
        private float yaw;
        private float pitch;

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Bootup(byte code)
        {
            base.Bootup(code);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            yaw = yawTarget.localEulerAngles.y;
            pitch = pitchTarget.localEulerAngles.x;
        }
        public override void Shutdown(byte code)
        {
            base.Shutdown(code);
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            var euler = yawTarget.localEulerAngles;
            var yawInput = Inputs.x;
            yawInput += yawAcceleration.Process(yawInput);
            yaw += yawKnob.Process(yawInput) * Time.deltaTime;
            
            euler.y = yaw;
            yawTarget.localEulerAngles = euler;

            euler = pitchTarget.localEulerAngles;
            pitch += pitchKnob.Process(-Inputs.y) * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchRange.x, pitchRange.y);
            
            euler.x = pitch;
            pitchTarget.localEulerAngles = euler;
        }
    }
}