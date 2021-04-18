using System;
using Flux;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Life : IDamageable, IBootable
    {
        [SerializeField] private Lifetime link;
        [SerializeField] private float maxHealth;

        private float health;

        public void Bootup() => health = maxHealth;
        
        public void Hit(RaycastHit hit, float damage)
        {
            health -= damage;
            if (health <= 0) link.End();
        }
    }
}