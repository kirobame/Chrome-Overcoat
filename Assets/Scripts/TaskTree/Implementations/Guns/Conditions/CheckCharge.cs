using System;
using System.Collections;
using Flux;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CheckCharge : Condition
    {
        public CheckCharge(float minimum, float decrease)
        {
            this.minimum = minimum;
            this.decrease = decrease;
        }

        [SerializeField] private float minimum;
        [SerializeField] private float decrease;
        
        private Coroutine cooldownRoutine;

        public override void Bootup(Packet packet)
        {
            var identity = packet.Get<IIdentity>();
            if (identity.Faction == Faction.Player)
            {
                var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
                HUD.IndicateThreshold(minimum);
            }
        }

        public override bool Check(Packet packet)
        {
            if (cooldownRoutine != null) return false;
            
            var board = packet.Get<IBlackboard>();
            var charge = board.Get<float>("charge");
            
            if (charge >= minimum)
            {
                cooldownRoutine = Routines.Start(CooldownRoutine(packet, board, charge));
                return true;
            }
            else
            {
                board.Set("charge.isUsed", false);
                board.Set("charge", 0.0f);

                var identity = packet.Get<IIdentity>();
                if (identity.Faction == Faction.Player)
                {
                    var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
                    HUD.Set(0.0f);
                }
                
                return false;
            }
        }
        
        private IEnumerator CooldownRoutine(Packet packet, IBlackboard board, float charge)
        {
            var identity = packet.Get<IIdentity>();
            var updateHUD = identity.Faction == Faction.Player;

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
                if (updateHUD)
                {
                    var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
                    HUD.Set(charge);
                }
            }

            cooldownRoutine = null;
        }
    }
}