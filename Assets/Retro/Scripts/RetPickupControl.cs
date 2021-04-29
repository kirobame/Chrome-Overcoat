using System;
using System.Linq;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPickupControl : MonoBehaviour
    {
        public event Action onPickupLost;
        public event Action<RetGunPickup> onPickupFound;
        
        [FoldoutGroup("Values"), SerializeField] private float radius;
        
        [FoldoutGroup("Dependencies"), SerializeField] private RetGunControl gun;

        private bool hasPickup;
        private RetGunPickup pickup;

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Z)) return;

            if (hasPickup)
            {
                gun.SwitchTo(pickup.Gun, pickup.ammo);
                
                OnPickupLost();
                hasPickup = false;
            
                pickup.gameObject.SetActive(false);
            }
            else if (!gun.IsOnDefault) gun.DropCurrent();
        }
        
        void FixedUpdate()
        {
            var center = transform.position.Flatten();
            var results = Physics.OverlapSphere(center, radius, LayerMask.GetMask("RetPickup"));
            
            if (!results.Any())
            {
                if (!hasPickup) return;

                OnPickupLost();
                hasPickup = false;
            }
            else
            {
                var match = false;
                var newPickup = default(RetGunPickup);
                var minDistance = float.PositiveInfinity;
                
                foreach (var result in results)
                {
                    if (!result.TryGetComponent<RetGunPickup>(out var foundPickup)) continue;

                    var distance = Vector3.Distance(foundPickup.transform.position.Flatten(), center);
                    if (distance < minDistance)
                    {
                        match = true;
                        minDistance = distance;
                        
                        newPickup = foundPickup;
                    }
                }

                if (match)
                {
                    if (hasPickup)
                    {
                        if (newPickup != pickup)
                        {
                            onPickupFound?.Invoke(newPickup);
                            Events.ZipCall(RetEvent.OnGunFound, pickup);
                        }
                    }
                    else
                    {
                        OnPickupFound(newPickup);
                        hasPickup = true;
                    }

                    pickup = newPickup;
                }
            }
        }

        private void OnPickupLost()
        {
            onPickupLost?.Invoke();
            Events.Call(RetEvent.OnGunLost);
        }
        private void OnPickupFound(RetGunPickup pickup)
        {
            onPickupFound?.Invoke(pickup);
            Events.ZipCall(RetEvent.OnGunFound, pickup);
        }
    }
}