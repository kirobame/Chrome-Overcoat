using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Chrome
{
    public class ImpactControl : MonoBehaviour, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            
        }

        //--------------------------------------------------------------------------------------------------------------/

        [FoldoutGroup("Values"), SerializeField] private float knockback;
        [FoldoutGroup("Values"), SerializeField] private float factor;
        [FoldoutGroup("Values"), SerializeField] private float maxLength;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        [FoldoutGroup("Values"), SerializeField] private float reduction;

        private IValue<CharacterBody> body;
        
        private Vector3 anchor;
        private Vector3 force;

        private Vector3 previousVelocity;
        private bool previousIsGrounded;
        
        private Vector3 velocity;
        private Vector3 forceVelocity;
        
        void Awake()
        {
            body = new AnyValue<CharacterBody>();
            injections = new IValue[] { body };
            
            Events.Subscribe<float>(PlayerEvent.OnFire, OnFire);
            anchor = transform.localPosition;
        }
        void OnDestroy() => Events.Unsubscribe<float>(PlayerEvent.OnFire, OnFire);
        
        void Update()
        {
            if (!previousIsGrounded && body.Value.IsGrounded)
            {
                force = Vector3.down * (Mathf.Abs(previousVelocity.y) * factor);
                Add(force);
            }

            previousVelocity = body.Value.Delta;
            previousIsGrounded = body.Value.IsGrounded;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, anchor + force, ref velocity, smoothing);
            force = Vector3.SmoothDamp(force, Vector3.zero, ref forceVelocity, reduction);
        }

        private void Add(Vector3 value)
        {
            force += value;
            if (force.magnitude > maxLength) force = force.normalized * maxLength;
        }

        void OnFire(float force)
        {
            var direction = new Vector3(0.0f, -transform.forward.y, -1.0f).normalized;
            Add(direction * (force * maxLength * knockback));
        }
    }
}