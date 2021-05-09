using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class SwaveControl : TiltControl
    {
        [FoldoutGroup("Pitch"), SerializeField] private float pitchViewAffect;

        [FoldoutGroup("Yaw"), SerializeField] private Knob yawKnob;
        [FoldoutGroup("Yaw"), SerializeField] private float yawViewAffect;

        private IValue<ViewControl> view;

        protected override void Awake()
        {
            base.Awake();
            
            view = new AnyValue<ViewControl>();
            injections.Add(view);
        }
        
        protected override float ComputePitch(Vector3 bodyDelta)
        {
            bodyDelta.y += view.Value.Delta.y * pitchViewAffect;
            return base.ComputePitch(bodyDelta);
        }
        protected override float ComputeYaw(Vector3 bodyDelta) => yawKnob.Process(view.Value.Delta.x * yawViewAffect);
    }
}