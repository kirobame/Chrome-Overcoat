using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class MoveHandler : MonoBehaviour
    {
        public Vector3 Inputs { get; private set; }
        public Vector3 Direction { get; private set; }
        
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        
        [BoxGroup("Values"), SerializeField] private float speed;
        [BoxGroup("Values"), SerializeField] private float smoothing;
        
        [BoxGroup("Airborne"), SerializeField, Range(0.0001f, 1.0f)] private float airSmoothing;
        [BoxGroup("Airborne"), SerializeField] private AnimationCurve switchMap;
        [BoxGroup("Airborne"), SerializeField] private float switchTime;
        
        private float switchProgress;
        private MoveControl airControl;
        private MoveControl groundControl;

        void Update()
        {
            if (body.IsGrounded) SwitchControl(MoveState.Grounded, 1.0f);
            else
            {
                switchProgress += Time.deltaTime;
                switchProgress = Mathf.Clamp(switchProgress, 0.0f, switchTime);
            }
            
            Inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
            Direction = body.transform.TransformVector(Inputs);

            var delta = Direction * speed;
            
            var airRatio = switchMap.Evaluate(switchProgress / switchTime);
            body.intent += airControl.Process(delta * airRatio, smoothing * airSmoothing);
            
            var groundRatio = 1.0f - airRatio;
            body.move += groundControl.Process(delta * groundRatio, smoothing);
        }

        public void SwitchControl(MoveState state, float ratio)
        {
            if (state == MoveState.Grounded) switchProgress = switchTime * (1.0f - ratio);
            else switchProgress = switchTime * ratio;
        }
    }
}