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
        
        public IPickable ClosestPickable { get; private set; }
        public bool CanPickup { get; private set; }
        
        [FoldoutGroup("Values"), SerializeField] private float range;
        
        private CachedValue<Key> key;
        
        private IValue<IIdentity> identity;
        private IValue<GunControl> gun;

        private HashSet<IPickable> pickables;

        void Awake()
        {
            pickables = new HashSet<IPickable>();
            Repository.Set(Reference.Pickups, new List<IPickable>());
        }

        void Update()
        {
            CanPickup = false;
            ClosestPickable = null;
            
            var shortestDistance = float.MaxValue;
            foreach (var pickable in Repository.GetAll<IPickable>(Reference.Pickups))
            {
                var distance = Vector3.Distance(transform.position, pickable.Transform.position);
                if (distance <= range)
                {
                    CanPickup = true;
                    if (distance < shortestDistance)
                    {
                        ClosestPickable = pickable;
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
            
            if (!key.IsDown()) return;

            if (CanPickup)
            {
                ClosestPickable.Pickup(identity.Value);
                pickables.Remove(ClosestPickable);
            }
            else if (!gun.Value.HasDefaultWeapon) gun.Value.DropCurrent();
        }
    }
}