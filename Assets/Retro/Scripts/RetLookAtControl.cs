using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetLookAtControl : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private RetMoveControl move;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform pivot;
        
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private Vector3 inputs;
        private float smoothedAngle;
        private float damping;
        
        void Update()
        {
            if (move.Inputs != Vector3.zero) inputs = move.Inputs;
            
            var angle = Vector3.SignedAngle(Vector3.forward, inputs, Vector3.up);
            smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, angle, ref damping, smoothing);
            
            transform.root.eulerAngles = new Vector3(0.0f, smoothedAngle, 0.0f);
            pivot.transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);
        }
    }
}