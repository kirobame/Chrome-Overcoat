using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetGunControl : MonoBehaviour, ILifebound, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;
        
        [ShowInInspector, HideInEditorMode] public RetGun Current { get; private set; }

        [FoldoutGroup("Dependencies"), SerializeField] private Transform modelParent;
        [FoldoutGroup("Dependencies"), SerializeField] private RetDetectionControl detection;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform aim;

        [FoldoutGroup("Values"), SerializeField] private RetGun defaultGun;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;
        
        private float smoothedAngle;
        private float damping;
        
        private Coroutine routine;

        void Awake()
        {
            Current = defaultGun;
            detection.onTargetEntry += OnTargetEntry;
        }
        void Start() => InstantiateModel();
        void OnDestroy() => detection.onTargetEntry -= OnTargetEntry;

        public void Bootup() { }
        public void Shutdown() => DropCurrent();
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (routine == null) smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, 0.0f, ref damping, smoothing);
            aim.localRotation = Quaternion.Euler(0.0f, smoothedAngle, 0.0f);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public void DropCurrent() => SwitchTo(defaultGun);
        public void SwitchTo(RetGun gun)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
                
                Current.Interrupt();
                Execute();

                AttemptNewFiring();
            }
            else Execute();

            void Execute()
            {
                var model = modelParent.GetComponentInChildren<RetGunModel>();
                if (model != null)
                {
                    var children = new Transform[model.transform.childCount];
                    for (var i = 0; i < children.Length; i++) children[i] = model.transform.GetChild(i);
                    
                    foreach (var child in children)
                    {
                        child.gameObject.SetActive(false);
                        child.SetParent(null);
                    }
                    Destroy(model.gameObject);
                }
                
                Current = gun;
                InstantiateModel();
            }
        }

        private void InstantiateModel()
        {
            var newModel = Instantiate(Current.Model, modelParent);
            var board = identity.Packet.Get<IBlackboard>();
            
            board.Set(RetPlayerBoard.REF_FIREANCHOR, newModel.FireAnchor);
            identity.Packet.Set(newModel.FireAnchor);
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        private IEnumerator Routine(Collider target, InteractionHub hub)
        {
            Vector3 direction;
            ComputeDirection();
            
            Current.Begin(identity, target, hub);
            while (!Current.Use(identity, target, hub))
            {
                ComputeDirection();
                
                var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
                smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, angle, ref damping, smoothing);
                
                yield return new WaitForEndOfFrame();
            }
            Current.End(identity, target, hub);
            
            void ComputeDirection()
            {
                direction = Vector3.Normalize(target.transform.position.Flatten() - transform.position.Flatten());
                identity.Packet.Set(direction);
                
                direction = transform.InverseTransformDirection(direction);
            }
            
            if (AttemptNewFiring()) yield break;
            routine = null;
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        private bool IsValid(Collider collider, out InteractionHub interactionHub)
        {
            return collider.TryGetComponent<InteractionHub>(out interactionHub) && interactionHub.Identity.Faction != identity.Faction;
        }
        private bool AttemptNewFiring()
        {
            if (detection.Targets.Any())
            {
                foreach (var newTarget in detection.Targets)
                {
                    if (!IsValid(newTarget, out var interactionHub)) continue;
                    
                    routine = StartCoroutine(Routine(newTarget, interactionHub));
                    return true;
                }
            }

            return false;
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnTargetEntry(Collider target)
        {
            if (routine != null || !IsValid(target, out var interactionHub)) return;
            routine = StartCoroutine(Routine(target, interactionHub));
        }
    }
}