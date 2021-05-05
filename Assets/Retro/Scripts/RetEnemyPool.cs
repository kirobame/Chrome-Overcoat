using System;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome.Retro
{
    public class RetEnemyPool : Pool<Identity, RetPoolableEnemy>
    {
        #region Nested Types

        [Serializable]
        private class EnemyProvider : Provider<Identity, RetPoolableEnemy> { }

        #endregion

        protected override IList<Provider<Identity, RetPoolableEnemy>> Providers => providers;
        [SerializeField] private EnemyProvider[] providers;
    }
}