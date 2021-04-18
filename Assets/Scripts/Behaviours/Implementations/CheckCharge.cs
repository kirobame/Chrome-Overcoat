using System;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CheckCharge : ConditionalNode
    {
        public CheckCharge(float minimum)
        {
            this.minimum = minimum;
            
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.IndicateThreshold(minimum);
        }

        [SerializeField] private float minimum;
        
        protected override bool Check(Packet packet)
        {
            var board = packet.Get<Blackboard>();
            return board.Get<float>("charge") >= minimum;
        }
    }
}