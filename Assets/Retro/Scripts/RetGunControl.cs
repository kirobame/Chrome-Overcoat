using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunControl : MonoBehaviour, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        [FoldoutGroup("Dependencies"), SerializeField] private RetDetectionControl detection;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform aim;

        [FoldoutGroup("Values"), SerializeField] private RetGun gun;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        
        private float smoothedAngle;
        private float damping;
        
        private Coroutine routine;
        
        void Awake() => detection.onTargetEntry += OnTargetEntry;
        void OnDestroy() => detection.onTargetEntry -= OnTargetEntry;

        void Update()
        {
            if (routine == null) smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, 0.0f, ref damping, smoothing);
            aim.localRotation = Quaternion.Euler(0.0f, smoothedAngle, 0.0f);
        }
        
        private IEnumerator Routine(Collider target, InteractionHub hub)
        {
            Vector3 direction;
            ComputeDirection();
            gun.Begin(identity, target, hub);
            
            while (!gun.Use(identity, target, hub))
            {
                ComputeDirection();
                
                var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
                smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, angle, ref damping, smoothing);
                
                yield return new WaitForEndOfFrame();
            }

            void ComputeDirection()
            {
                direction = Vector3.Normalize(target.transform.position.Flatten() - transform.position.Flatten());
                identity.Packet.Set(direction);
                
                direction = transform.InverseTransformDirection(direction);
            }
            
            gun.End(identity, target, hub);

            if (detection.Targets.Any())
            {
                foreach (var newTarget in detection.Targets)
                {
                    if (!IsValid(newTarget, out var interactionHub)) continue;
                    
                    routine = StartCoroutine(Routine(newTarget, interactionHub));
                    break;
                }
            }
            
            routine = null;
        }

        private bool IsValid(Collider collider, out InteractionHub interactionHub)
        {
            return collider.TryGetComponent<InteractionHub>(out interactionHub) && interactionHub.Identity.Faction != identity.Faction;
        }

        void OnTargetEntry(Collider target)
        {
            if (routine != null || !IsValid(target, out var interactionHub)) return;
            routine = StartCoroutine(Routine(target, interactionHub));
        }
    }
}