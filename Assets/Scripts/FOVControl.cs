using System;
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
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve viewMap;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve distortionMap;
        [FoldoutGroup("Values"), SerializeField] private float groundSmoothing;
        [FoldoutGroup("Values"), SerializeField] private float airSmoothing;

        private float current;
        private float velocity;
        
        void Update()
        {
            var velocity = body.transform.InverseTransformVector(body.Controller.velocity);
            var speed = Mathf.Clamp(velocity.z, velocityMap.keys[0].value, velocityMap.keys.Last().value);
            var target = velocityMap.Evaluate(speed);
            
            current = Mathf.SmoothDamp(current, target, ref this.velocity, body.IsGrounded ? groundSmoothing : airSmoothing);

            var volume = Repository.Get<UnityEngine.Rendering.Volume>(Volume.Run);
            if (volume.profile.TryGet<LensDistortion>(out var distortion)) distortion.intensity.value = distortionMap.Evaluate(current);
            
            camera.fieldOfView = viewMap.Evaluate(current);
        }
    }
}