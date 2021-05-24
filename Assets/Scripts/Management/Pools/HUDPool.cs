using System;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class HUDPool : Pool<HUD, PoolableHUD>
    {
        #region Nested types

        [Serializable]
        private class HUDProvider : Provider<HUD, PoolableHUD> { }

        #endregion
        
        protected override IList<Provider<HUD, PoolableHUD>> Providers => providers;
        [SerializeField] private HUDProvider[] providers;

        protected override void OnReady()
        {
            foreach (var provider in providers) HUDBinder.RegisterHUDPrefab(provider.Prefab);
        }
    }
}