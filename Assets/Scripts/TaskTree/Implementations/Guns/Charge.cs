using System;
using System.Collections;
using Flux;
using Flux.Data;
using Flux.Event;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Charge : TaskNode
    {
        public Charge(BindableCappedGauge gaugeBinding, float shakeFactor, float maxShake)
        {
            this.gaugeBinding = gaugeBinding;

            this.shakeFactor = shakeFactor;
            this.maxShake = maxShake;
        }
        
        [SerializeField] private float shakeFactor;
        [SerializeField] private float maxShake;

        private BindableCappedGauge gaugeBinding;

        protected override void OnUse(Packet packet)
        {
            if (gaugeBinding.IsLocked) return;
            
            gaugeBinding.Value += Time.deltaTime;

            var identity = packet.Get<IIdentity>();
            if (identity.Faction == Faction.Player) Events.ZipCall(PlayerEvent.OnShake,  gaugeBinding.ComputeRatio() * shakeFactor, maxShake);
            
            isDone = true;
        }
    }
}