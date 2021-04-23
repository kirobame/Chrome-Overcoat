using System;
using System.Collections;
using Flux;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CheckCharge : ConditionalNode
    {
        public CheckCharge(float minimum, float decrease)
        {
            this.minimum = minimum;
            this.decrease = decrease;
            
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.IndicateThreshold(minimum);
        }

        [SerializeField] private float minimum;
        [SerializeField] private float decrease;
        
        private Coroutine cooldownRoutine;
        
        protected override bool Check(Packet packet)
        {
            if (cooldownRoutine != null) return false;
            
            var board = packet.Get<IBlackboard>();
            var charge = board.Get<float>("charge");
            
            if (charge >= minimum)
            {
                cooldownRoutine = Routines.Start(CooldownRoutine(board, charge));
                return true;
            }
            else
            {
                board.Set("charge.isUsed", false);
                board.Set("charge", 0.0f);
                
                var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
                HUD.Set(0.0f);
                
                return false;
            }
        }
        
        private IEnumerator CooldownRoutine(IBlackboard board, float charge)
        {
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            
            while (charge > 0.0f)
            {
                charge -= Time.deltaTime * decrease;
                Execute();
                
                yield return new WaitForEndOfFrame();
            }

            charge = 0.0f;
            Execute();

            void Execute()
            {
                board.Set("charge", charge);
                HUD.Set(charge);
            }

            cooldownRoutine = null;
        }
    }
}