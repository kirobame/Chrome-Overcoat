using System;
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
            
        public override void Reboot() => onDeath?.Invoke(this);
    }
}