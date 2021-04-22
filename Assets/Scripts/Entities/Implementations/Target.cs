using System;
using System.Collections;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Target : MonoBehaviour, IHittable
    {
        public IExtendedIdentity Identity { get; private set; }
        public bool IsAlive => health > 0.0f;
        
        [BoxGroup("Dependencies"), SerializeField] private new Collider collider;
        
        [FoldoutGroup("Values"), SerializeField] private float maxHealth;
        [FoldoutGroup("Values"), SerializeField] private Color minColor;
        [FoldoutGroup("Values"), SerializeField] private Color maxColor;
        [FoldoutGroup("Values"), SerializeField] private float respawnTime;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private Animator animator;
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx hitVfx;
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx deathVfx;

        private float health;

        void Awake()
        {
            Identity = new PackedIdentity(Faction.Enemy, transform);
            health = maxHealth;
        }

        public void Hit(IIdentity identity, HitMotive motive, EventArgs args)
        {
            if (motive != HitMotive.Damage || !(args is PointDamageArgs damageArgs)) return;
            
            health -= damageArgs.Amount;
            
            var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
            var hitVfxInstance = vfxPool.RequestSingle(hitVfx);
            
            var module = hitVfxInstance.main;
            var gradient = module.startColor;
            gradient.color =  Color.Lerp(maxColor, minColor, Mathf.Clamp01(health / maxHealth));
            module.startColor = gradient;

            hitVfxInstance.transform.position = transform.position;
            hitVfxInstance.transform.rotation = Quaternion.LookRotation(damageArgs.Hit.normal);
            hitVfxInstance.Play();

            if (identity.Faction == Faction.Player)
            {
                Events.ZipCall<byte,float>(GaugeEvent.OnDamageInflicted, 0, damageArgs.Amount);
                if (health <= 0.0f) Events.ZipCall<byte>(GaugeEvent.OnKill, 0);
            }
            
            if (health <= 0.0f) StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(false);
            }

            animator.SetTrigger("In");
            collider.enabled = false;
            
            yield return new WaitForSeconds(0.125f);
            
            var vfxPool = Repository.Get<VfxPool>(Pool.Death);
            var deathVfxInstance = vfxPool.RequestSingle(deathVfx);
            
            deathVfxInstance.transform.position = transform.position;
            deathVfxInstance.Play();
            
            yield return new WaitForSeconds(respawnTime);
            
            animator.SetTrigger("Out");
            yield return new WaitForSeconds(0.5f);

            collider.enabled = true;
            health = maxHealth;
        }
    }
}