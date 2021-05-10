using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class ViewControl : InputControl<ViewControl>
    {
        public Vector2 Inputs { get; private set; }
        public Vector2 Delta => new Vector2(yawKnob.Value, pitchKnob.Value);
        
        [FoldoutGroup("Yaw"), SerializeField] private Transform yawTarget;
        [FoldoutGroup("Yaw"), SerializeField] private Knob yawKnob;
        [FoldoutGroup("Yaw"), SerializeField] private Acceleration yawAcceleration;
     
        [FoldoutGroup("Pitch"), SerializeField] private Transform pitchTarget;
        [FoldoutGroup("Pitch"), SerializeField] private Knob pitchKnob;
        [FoldoutGroup("Pitch"), SerializeField] private Vector2 pitchRange;
        
        private float yaw;
        private float pitch;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            base.Awake();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            yaw = yawTarget.localEulerAngles.y;
            pitch = pitchTarget.localEulerAngles.x;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        protected override void SetupInputs() => input.Value.Bind(InputRefs.VIEW, this, OnViewInput);
        void OnViewInput(InputAction.CallbackContext context, InputCallbackType type) => Inputs = context.ReadValue<Vector2>();

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