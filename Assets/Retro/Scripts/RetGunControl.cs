using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chrome.Retro
{
    public class RetGunControl : RetBaseGunControl, ILifebound, ILink<IIdentity>
    {
        IIdentity ILink<IIdentity>.Link
        {
            set => identity = value;
        }
        private IIdentity identity;

        public override bool IsOnDefault => Current == defaultGun;
        
        [FoldoutGroup("Dependencies"), SerializeField] private Transform modelParent;
        [FoldoutGroup("Dependencies"), SerializeField] private RetDetectionControl detection;
        [FoldoutGroup("Dependencies"), SerializeField] private Transform aim;
        [FoldoutGroup("Dependencies"), SerializeField] private Animator animator;

        [FoldoutGroup("Values"), SerializeField] private RetGun defaultGun;
        [FoldoutGroup("Values"), SerializeField] private float smoothing;

        private RetGunModel model;
        private int ammo;
        
        private float smoothedAngle;
        private float damping;
        
        private Coroutine routine;

        void Awake()
        {
            if (!enabled) return;

            Current = defaultGun;
            detection.onTargetEntry += OnTargetEntry;
        }
        void Start() => InstantiateModel();
        void OnDestroy() => detection.onTargetEntry -= OnTargetEntry;

        public void Bootup()
        {
            if (!enabled) return;
            ActualizeMesh();
        }
        public void Shutdown()
        {
            if (!enabled) return;
            DropCurrent();
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        void Update()
        {
            if (routine == null) smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, 0.0f, ref damping, smoothing);
            //aim.localRotation = Quaternion.Euler(0.0f, smoothedAngle, 0.0f);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void DropCurrent() => SwitchTo(defaultGun, -1);
        public override void SwitchTo(RetGun gun, int ammo)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
                
                Current.Interrupt();
                Execute();
            }
            else Execute();

            void Execute()
            {
                if (Current != defaultGun && this.ammo > 0)
                {
                    var pickupPool = Repository.Get<GenericPool>(RetReference.PickupPool);
                    var pickupInstance = pickupPool.CastSingle<RetGunPickup>(Current.Pickup);
                    pickupInstance.ammo = this.ammo;

                    var angle = Random.Range(70.0f, 80.0f) * Mathf.Deg2Rad;
                    var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
            
                    var y = Random.Range(0.0f, 360.0f);
                    direction = Vector3.Normalize(Quaternion.AngleAxis(y, Vector3.up) * direction);
            
                    pickupInstance.transform.position = aim.position;
                    pickupInstance.Rigidbody.velocity = Vector3.zero;
                    pickupInstance.Rigidbody.AddForce(direction * Random.Range(12.5f, 17.5f), ForceMode.Impulse);
                    pickupInstance.Rigidbody.AddTorque(Random.onUnitSphere * Random.Range(7.5f, 12.5f), ForceMode.Impulse);
                }
                model.Discard();
                
                Current = gun;
                this.ammo = ammo;
                
                SignalSwitch(gun);
                Events.ZipCall<RetGun,int>(RetEvent.OnGunSwitch, Current, ammo);
                
                InstantiateModel();
                ActualizeMesh();
                
                if (gameObject.activeInHierarchy) AttemptNewFiring();
            }
        }

        private void InstantiateModel()
        {
            Debug.Log(Current);
            model = Instantiate(Current.Model, modelParent);
            var board = identity.Packet.Get<IBlackboard>();
            
            board.Set(RetPlayerBoard.REF_FIREANCHOR, model.FireAnchor);
            identity.Packet.Set(model.FireAnchor);
        }
        private void ActualizeMesh()
        {
            if (Current == defaultGun)
            {
                animator.SetBool("IsMelee", true);
                animator.SetBool("IsRange", false);
            }
            else
            {
                animator.SetBool("IsMelee", false);
                animator.SetBool("IsRange", true);
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        private IEnumerator Routine(Collider target, InteractionHub hub)
        {
            Vector3 direction;
            ComputeDirection();

            if (ammo > 0)
            {
                ammo--;
                Events.ZipCall<int>(RetEvent.OnAmmoChange, ammo);
            }
            
            Current.Begin(identity, target, hub);
            while (!Current.Use(identity, target, hub))
            {
                ComputeDirection();
                
                var angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
                smoothedAngle = Mathf.SmoothDampAngle(smoothedAngle, angle, ref damping, smoothing);
                
                yield return new WaitForEndOfFrame();
            }
            Current.End(identity, target, hub);

            if (ammo == 0)
            {
                routine = null;
                DropCurrent();
                    
                yield break;
            }
            
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