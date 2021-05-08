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
        
        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;

        [SerializeField] private GaugeResetInfo[] resetInfos;
         
        void Start() => ResetGauges();
        void OnDestroy() => onDestruction?.Invoke(this);

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