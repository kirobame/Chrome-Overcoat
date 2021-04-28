﻿using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetPickupControl : MonoBehaviour
    {
        public event Action onLeftPickup;
        public event Action<RetGunPickup> onPickupFound;
        
        [FoldoutGroup("Values"), SerializeField] private float radius;
        
        [FoldoutGroup("Dependencies"), SerializeField] private RetGunControl gun;
        [FoldoutGroup("Dependencies"), SerializeField] private GameObject indicator;

        private bool hasPickup;
        private RetGunPickup pickup;

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.P) || !hasPickup) return;
            
            gun.SwitchTo(pickup.Gun);

            hasPickup = false;
            Destroy(pickup.gameObject);
        }
        
        void FixedUpdate()
        {
            var center = transform.position.Flatten();
            var results = Physics.OverlapSphere(center, radius, LayerMask.GetMask("RetPickup"));
            
            if (!results.Any())
            {
                if (!hasPickup) return;
                
                indicator.SetActive(false);
                onLeftPickup?.Invoke();

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
                        if (newPickup != pickup) onPickupFound?.Invoke(newPickup);
                    }
                    else
                    {
                        onPickupFound?.Invoke(newPickup);
                        
                        indicator.SetActive(true);
                        hasPickup = true;
                    }

                    pickup = newPickup;
                }
            }
        }
    }
}