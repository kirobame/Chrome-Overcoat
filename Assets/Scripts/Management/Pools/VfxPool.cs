using System;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class VfxPool : Pool<ParticleSystem, PoolableVfx>
    {
        #region Nested Types

        [Serializable]
        private class VfxProvider : Provider<ParticleSystem,PoolableVfx> { }

        #endregion

        protected override IList<Provider<ParticleSystem, PoolableVfx>> Providers => providers;
        [SerializeField] private VfxProvider[] providers;
    }
}