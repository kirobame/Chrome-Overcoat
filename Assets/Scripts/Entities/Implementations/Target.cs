using System;
using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class Target : MonoBehaviour, IDamageable, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        //--------------------------------------------------------------------------------------------------------------/

        public event Action<IInteraction> onDestruction;
        
        public IIdentity Identity => identity.Value;
        public bool IsAlive => health > 0.0f;
        public bool IsActive => enabled;

        [BoxGroup("Dependencies"), SerializeField] private new Collider collider;
        
        [FoldoutGroup("Values"), SerializeField] private float maxHealth;
        [FoldoutGroup("Values"), SerializeField] private Color minColor;
        [FoldoutGroup("Values"), SerializeField] private Color maxColor;
        [FoldoutGroup("Values"), SerializeField] private float respawnTime;
        
        [FoldoutGroup("Feedbacks"), SerializeField] private Animator animator;
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx hitVfx;
        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx deathVfx;

        private IValue<IIdentity> identity;
        private float health;

        //--------------------------------------------------------------------------------------------------------------/
        
        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
            
            health = maxHealth;
        }
        void OnDestroy() => onDestruction?.Invoke(this);
        
        public void Hit(IIdentity source, float damage, Packet packet)
        {
            health -= damage;

            if (packet.TryGet<CollisionHit<Transform>>(out var hit))
            {
                var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
                var hitVfxInstance = vfxPool.RequestSingle(hitVfx);
            
                var module = hitVfxInstance.main;
                var gradient = module.startColor;
                gradient.color =  Color.Lerp(maxColor, minColor, Mathf.Clamp01(health / maxHealth));
                module.startColor = gradient;

                hitVfxInstance.transform.position = transform.position;
                hitVfxInstance.transform.rotation = Quaternion.LookRotation(hit.Normal);
                hitVfxInstance.Play();
            }
            
            if (source.Faction == Faction.Player)
            {
                var type = identity.Value.Packet.Get<byte>();
                
                Events.ZipCall<byte,float>(GaugeEvent.OnDamageInflicted, type, damage);
                if (health <= 0.0f) Events.ZipCall<byte>(GaugeEvent.OnKill, type);
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