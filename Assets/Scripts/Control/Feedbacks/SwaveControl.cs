using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class SwaveControl : TiltControl, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        //--------------------------------------------------------------------------------------------------------------/

        [FoldoutGroup("Pitch"), SerializeField] private float pitchViewAffect;

        [FoldoutGroup("Yaw"), SerializeField] private Knob yawKnob;
        [FoldoutGroup("Yaw"), SerializeField] private float yawViewAffect;

        private IValue<ViewControl> view;

        void Awake()
        {
            view = new AnyValue<ViewControl>();
            injections = new IValue[] { view };
        }
        
        protected override float ComputePitch(Vector3 bodyDelta)
        {
            bodyDelta.y += view.Value.Delta.y * pitchViewAffect;
            return base.ComputePitch(bodyDelta);
        }
        protected override float ComputeYaw(Vector3 bodyDelta) => yawKnob.Process(view.Value.Delta.x * yawViewAffect);
    }
}