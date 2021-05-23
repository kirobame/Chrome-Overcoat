using System;
using Flux;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class SpawnVfxControl : MonoBehaviour, IListener<Lifetime>
    {
        public event Action<IListener<Lifetime>> onDestruction;

        public bool IsActive => enabled;

        [SerializeField] private Pool address;
        [SerializeField] private PoolableVfx prefab;

        void OnDestroy() => onDestruction?.Invoke(this);

        public bool IsListeningTo(EventArgs args) => Lifetime.IsShutdownMessage(args);

        public void Execute(Token token)
        {
            var pool = Repository.Get<VfxPool>(address);
            var instance = pool.RequestSingle(prefab);

            instance.transform.position = transform.position;
            instance.Play();
            
            token.Consume();
        }
    }
}