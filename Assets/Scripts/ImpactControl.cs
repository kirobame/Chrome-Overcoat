using System.Collections;
using System.Linq;
using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Chrome
{
    public class ImpactControl : MonoBehaviour
    {
        [BoxGroup("Dependencies"), SerializeField] private PhysicBody body;
        [BoxGroup("Dependencies"), SerializeField] private MoveControl move;

        [FoldoutGroup("Values"), SerializeField] private float factor;
        [FoldoutGroup("Values"), SerializeField] private float maxLength;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        [FoldoutGroup("Values"), SerializeField] private float reduction;
        [FoldoutGroup("Values"), SerializeField] private AnimationCurve speedLossMap;

        private Vector3 anchor;
        private Vector3 force;

        private Vector3 previousVelocity;
        private bool previousIsGrounded;
        
        private Vector3 velocity;
        private Vector3 forceVelocity;

        private Coroutine speedLossRoutine;
        
        void Awake() => anchor = transform.localPosition;
        
        void Update()
        {
            if (!previousIsGrounded && body.IsGrounded)
            {
                force = Vector3.down * (Mathf.Abs(previousVelocity.y) * factor);
                if (force.magnitude > maxLength) force = force.normalized * maxLength;
                
                if (speedLossRoutine != null) StopCoroutine(speedLossRoutine);
                speedLossRoutine = StartCoroutine(SpeedLossRoutine(1.0f - force.magnitude / maxLength));
            }

            previousVelocity = body.Controller.velocity;
            previousIsGrounded = body.IsGrounded;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, anchor + force, ref velocity, smoothing);
            force = Vector3.SmoothDamp(force, Vector3.zero, ref forceVelocity, reduction);
        }

        private IEnumerator SpeedLossRoutine(float startingRatio)
        {
            var goal = speedLossMap.keys.Last().time;
            var time = goal * startingRatio;

            while (time < goal)
            {
                move.speedModifier = speedLossMap.Evaluate(time);
                
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }

            move.speedModifier = 1.0f;
            speedLossRoutine = null;
        }
    }
}