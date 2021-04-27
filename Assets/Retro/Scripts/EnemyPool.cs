using System;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome.Retro
{
    public class EnemyPool : Pool<Identity, PoolableEnemy>
    {
        #region Nested Types

        [Serializable]
        private class EnemyProvider : Provider<Identity, PoolableEnemy> { }

        #endregion

        protected override IList<Provider<Identity, PoolableEnemy>> Providers => providers;
        [SerializeField] private EnemyProvider[] providers;
    }
}