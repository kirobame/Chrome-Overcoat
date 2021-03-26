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

        [FoldoutGroup("Values"), SerializeField] private float factor;
        [FoldoutGroup("Values"), SerializeField] private float maxLength;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        [FoldoutGroup("Values"), SerializeField] private float reduction;

        private Vector3 anchor;
        private Vector3 force;

        private Vector3 previousVelocity;
        private bool previousIsGrounded;
        
        private Vector3 velocity;
        private Vector3 forceVelocity;
        
        void Awake() => anchor = transform.localPosition;
        
        void Update()
        {
            if (!previousIsGrounded && body.IsGrounded)
            {
                force = Vector3.down * (Mathf.Abs(previousVelocity.y) * factor);
                if (force.magnitude > maxLength) force = force.normalized * maxLength;
            }

            previousVelocity = body.Controller.velocity;
            previousIsGrounded = body.IsGrounded;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, anchor + force, ref velocity, smoothing);
            force = Vector3.SmoothDamp(force, Vector3.zero, ref forceVelocity, reduction);
        }
    }
}