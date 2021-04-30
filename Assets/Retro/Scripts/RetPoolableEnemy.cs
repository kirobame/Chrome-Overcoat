﻿using System;
using Flux;
using Flux.Audio;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Chrome.Retro
{
    public class RetPoolableEnemy : Poolable<Identity>
    {
        public Action<RetPoolableEnemy> onDeath;

        public NavMeshAgent Agent => agent;
        public Lifetime Lifetime => lifetime;

        [FoldoutGroup("Dependencies"), SerializeField] private Lifetime lifetime;
        [FoldoutGroup("Dependencies"), SerializeField] private NavMeshAgent agent;

        [FoldoutGroup("Feedbacks"), SerializeField] private PoolableVfx apparitionPrefab;
        [FoldoutGroup("Feedbacks"), SerializeField] private Vector3 offset;
        [FoldoutGroup("Feedbacks"), SerializeField] private AudioPackage deathSound;

        void OnEnable()
        {
            Routines.Start(Routines.DoAfter(() =>
            {
                var vfxPool = Repository.Get<VfxPool>(Pool.Impact);
                var vfxInstance = vfxPool.RequestSingle(apparitionPrefab);

                vfxInstance.transform.position = transform.position + offset;
                vfxInstance.Play();
                
            }, new YieldFrame()));
        }

        public override void Reboot()
        {
            deathSound.Play();
            onDeath?.Invoke(this);
        }
    }
}