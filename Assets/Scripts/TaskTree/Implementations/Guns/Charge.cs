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
    public class Charge : TaskedNode
    {
        public Charge(float duration, float shakeFactor, float maxShake)
        {
            this.duration = duration;

            this.shakeFactor = shakeFactor;
            this.maxShake = maxShake;
        }

        [SerializeField] private float duration;
        [SerializeField] private float shakeFactor;
        [SerializeField] private float maxShake;

        private bool canExecute;
        private float timer;

        protected override void Open(Packet packet)
        {
            canExecute = false;
            timer = 0.0f;
        }

        protected override void OnUse(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            float charge;
            
            if (!canExecute)
            {
                charge = board.Get<float>("charge");

                if (charge <= 0)
                {
                    canExecute = true;
                    board.Set("charge.isUsed", true);
                }
                else return;
            }
            
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, duration);
            charge = timer / duration;

            board.Set("charge", charge);

            var identity = packet.Get<IIdentity>();
            if (identity.Faction == Faction.Player)
            {
                Events.ZipCall(PlayerEvent.OnShake, shakeFactor * charge, maxShake);
                var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
                HUD.Set(charge);
            }
            
            isDone = true;
        }

        protected override void OnShutdown(Packet packet)
        {
            var board = packet.Get<IBlackboard>();
            board.Remove("charge");
        }
    }
}