using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class GaugeControl : MonoBehaviour, ILifebound
    {
        #region Nested Types

        [Serializable]
        public struct GaugeResetInfo
        {
            public Gauges address;
            public float value;
            public bool inPercent;
        }

        #endregion

        [SerializeField] private GaugeResetInfo[] resetInfos;
         
        void Start() => ResetGauges();
        
        public void Bootup() { }
        public void Shutdown() => ResetGauges();

        private void ResetGauges()
        {
            foreach (var resetInfo in resetInfos)
            {
                var gauge = Repository.Get<Gauge>(resetInfo.address);
                gauge.ResetAt(resetInfo.value, resetInfo.inPercent);
            }
        }
    }
}