using System;
using System.Collections;
using Flux;
using Flux.Data;
using Flux.Event;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class GaugeCost : ConditionalNode
    {
        public GaugeCost(IValue<bool> costLock, IValue<float> cost, Gauge gauge)
        {
            this.costLock = costLock;
            this.cost = cost;
            this.gauge = gauge;
        }

        [SerializeField] private IValue<float> cost;
        [SerializeField] private Gauge gauge;
        [SerializeField] private IValue<bool> costLock;

        protected override bool Check(Packet packet)
        {
            if (!cost.IsValid(packet) || !costLock.IsValid(packet)) return false;
            if (costLock.Value && gauge.Value + cost.Value > gauge.Max) return false;

            Events.ZipCall(GaugeEvent.OnFrenzyAbilityUsed, (byte)0, cost.Value);
            return true;
        }
    }
}