using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class PickupControl : InputControl<PickupControl>
    {
        protected override void PrepareInjection()
        {
            identity = injections.Register(new AnyValue<IIdentity>());
            gun = injections.Register(new AnyValue<GunControl>());
        }

        protected override void SetupInputs()
        {
            key = new CachedValue<Key>(Key.Inactive);
            input.Value.BindKey(InputRefs.PICK_WP_02, this, key);
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public BindableState<IPickable> ClosestPickable { get; private set; }

        [FoldoutGroup("Values"), SerializeField] private float range;
        
        private CachedValue<Key> key;
        
        private IValue<IIdentity> identity;
        private IValue<GunControl> gun;

        private HashSet<IPickable> pickables;

        //--------------------------------------------------------------------------------------------------------------/

        void Awake()
        {
            pickables = new HashSet<IPickable>();
            Repository.Set(Reference.Pickups, new List<IPickable>());
        }

        void Start()
        {
            ClosestPickable = new BindableState<IPickable>(HUDBinding.Popup);
            HUDBinder.Declare(HUDGroup.Pickup, ClosestPickable);
        }

        void Update()
        {
            var canPickup = false;
            var closestPickable = default(IPickable);
            
            var shortestDistance = float.MaxValue;
            foreach (var pickable in Repository.GetAll<IPickable>(Reference.Pickups))
            {
                var distance = Vector3.Distance(transform.position, pickable.Transform.position) - pickable.Radius;
                if (distance <= 0.0f) distance = 0.0f;
                
                if (distance <= range)
                {
                    canPickup = true;
                    if (distance < shortestDistance)
                    {
                        closestPickable = pickable;
                        shortestDistance = distance;
                    }

                    if (!pickables.Contains(pickable))
                    {
                        pickable.OnHoverStart(identity.Value);
                        pickables.Add(pickable);
                    }
                }
                else if (pickables.Contains(pickable))
                {
                    pickable.OnHoverEnd(identity.Value);
                    pickables.Remove(pickable);
                }
            }
            
            if (canPickup) ClosestPickable.Set(closestPickable);
            else if (ClosestPickable.HasValue) ClosestPickable.Discard();
            
            if (!key.IsDown()) return;

            if (canPickup)
            {
                closestPickable.Pickup(identity.Value);
                pickables.Remove(closestPickable);
            }
            else if (!gun.Value.HasDefaultWeapon) gun.Value.DropCurrent();
        }
    }
}