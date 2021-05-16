using System;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class AgentPool : Pool<Agent, PoolableAgent>
    {
        #region Nested Types

        [Serializable]
        private class AgentProvider : Provider<Agent, PoolableAgent> { }

        #endregion

        protected override IList<Provider<Agent, PoolableAgent>> Providers => providers;
        [SerializeField] private AgentProvider[] providers;
    }
}