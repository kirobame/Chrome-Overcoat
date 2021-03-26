using System.Linq;
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
        [BoxGroup("Dependencies"), SerializeField] private new Camera camera;

        [FoldoutGroup("Values"), SerializeField] private AnimationCurve velocityMap;
        [FoldoutGroup("Values"), SerializeField] private Converter fieldConverter;
        [FoldoutGroup("Values"), SerializeField] private Converter distortionConverter;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private float current;
        private float velocity;
        
        void Update()
        {
            var velocity = body.transform.InverseTransformVector(body.Controller.velocity);
            var speed = Mathf.Clamp(velocity.z, velocityMap.keys[0].time, velocityMap.keys.Last().time);
            var target = velocityMap.Evaluate(speed);

            current = Mathf.SmoothDamp(current, target, ref this.velocity, smoothing);

            var volume = Repository.Get<UnityEngine.Rendering.Volume>(Volume.Run);
            if (volume.profile.TryGet<LensDistortion>(out var distortion)) distortion.intensity.value = distortionConverter.Process(current);

            camera.fieldOfView = fieldConverter.Process(current);
        }
    }
}