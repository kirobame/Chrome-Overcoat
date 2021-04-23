using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class SwaveControl : TiltControl
    {
        [BoxGroup("Dependencies"), SerializeField] private ViewControl view;
        
        [FoldoutGroup("Pitch"), SerializeField] private float pitchViewAffect;

        [FoldoutGroup("Yaw"), SerializeField] private Knob yawKnob;
        [FoldoutGroup("Yaw"), SerializeField] private float yawViewAffect;
        
        protected override float ComputePitch(Vector3 bodyDelta)
        {
            bodyDelta.y += view.Delta.y * pitchViewAffect;
            return base.ComputePitch(bodyDelta);
        }
        protected override float ComputeYaw(Vector3 bodyDelta) => yawKnob.Process(view.Delta.x * yawViewAffect);
    }
}