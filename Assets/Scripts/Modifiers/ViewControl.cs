using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class ViewControl : MonoBehaviour
    {
        [FoldoutGroup("Yaw"), SerializeField] private Transform yawTarget;
        [FoldoutGroup("Yaw"), SerializeField] private Knob yawKnob;
        [FoldoutGroup("Yaw"), SerializeField] private Acceleration yawAcceleration;
     
        [FoldoutGroup("Pitch"), SerializeField] private Transform pitchTarget;
        [FoldoutGroup("Pitch"), SerializeField] private Knob pitchKnob;
        [FoldoutGroup("Pitch"), SerializeField] private Vector2 pitchRange;

        private float yaw;
        private float pitch;
        
        void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            yaw = yawTarget.localEulerAngles.y;
            pitch = pitchTarget.localEulerAngles.x;
        }
        void OnDestroy()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        void Update()
        {
            var euler = yawTarget.localEulerAngles;
            var yawInput = Input.GetAxisRaw("Mouse X");
            //yawInput += yawAcceleration.Process(yawInput);
            yaw += yawKnob.Process(yawInput);
            
            euler.y = yaw;
            yawTarget.localEulerAngles = euler;

            euler = pitchTarget.localEulerAngles;
            pitch += pitchKnob.Process(-Input.GetAxisRaw("Mouse Y"));
            pitch = Mathf.Clamp(pitch, pitchRange.x, pitchRange.y);
            
            euler.x = pitch;
            pitchTarget.localEulerAngles = euler;
        }
    }
}