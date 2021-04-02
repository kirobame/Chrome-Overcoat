using System.Linq;
using Cinemachine;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chrome
{
    public class FOVControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        [BoxGroup("Dependencies"), SerializeField] private new CinemachineVirtualCamera camera;

        [FoldoutGroup("Values"), SerializeField] private AnimationCurve velocityMap;
        [FoldoutGroup("Values"), SerializeField] private Converter fieldConverter;
        [FoldoutGroup("Values"), SerializeField] private Converter distortionConverter;
        
        void Update()
        {
            var velocity = body.transform.InverseTransformVector(body.Controller.velocity);
            var speed = Mathf.Clamp(velocity.z, velocityMap.keys[0].time, velocityMap.keys.Last().time);
            var target = velocityMap.Evaluate(speed);
            
            var volume = Repository.Get<UnityEngine.Rendering.Volume>(Volume.Run);
            if (volume.profile.TryGet<LensDistortion>(out var distortion))
            {
                distortion.intensity.value = distortionConverter.Process(target);
                volume.profile.isDirty = true;
            }

            camera.m_Lens.FieldOfView = fieldConverter.Process(target);
        }
    }
}