using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetAgentLookAt : MonoBehaviour
    {
        [FoldoutGroup("Dependency"), SerializeField] private Transform pivot;
        
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        
        [HideInInspector] public Vector3 direction;

        private Vector3 smoothedDirection;
        private Vector3 damping;

        void Update()
        {
            smoothedDirection = Vector3.Normalize(Vector3.SmoothDamp(smoothedDirection, direction, ref damping, smoothing));
            pivot.transform.localRotation = Quaternion.LookRotation(smoothedDirection, pivot.transform.up);
        }
    }
}