using System.Linq;
using Cinemachine;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chrome
{
    public class FOVControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private CharacterBody body;
        [BoxGroup("Dependencies"), SerializeField] private new CinemachineVirtualCamera camera;

        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        [FoldoutGroup("Values"), SerializeField] private Vector2 input;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve FOVMap;
        [FoldoutGroup("Values"), SerializeField] private float FOVMidpoint;
        [FoldoutGroup("Values"), SerializeField] private float FOVFactor;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve distortionMap;
        [FoldoutGroup("Values"), SerializeField] private float distortionFactor;

        private float current;
        private float damping;
        
        void Update()
        {
            var forward = body.transform.InverseTransformVector(body.Delta).z;

            float ratio;
            if (forward < 0) ratio = Mathf.InverseLerp(input.x, 0.0f, forward) - 1.0f;
            else ratio = Mathf.InverseLerp(0.0f, input.y, forward);

            current = Mathf.SmoothDamp(current, ratio, ref damping, smoothing); 
            
            var volume = Repository.Get<UnityEngine.Rendering.Volume>(Volume.Run);
            if (volume.profile.TryGet<LensDistortion>(out var distortion))
            {
                distortion.intensity.value = distortionMap.Evaluate(current) * distortionFactor;
                volume.profile.isDirty = true;
            }

            camera.m_Lens.FieldOfView = FOVMidpoint + FOVMap.Evaluate(current) * FOVFactor;
        }
    }
}