﻿using System;
using System.Collections;
using Flux;
using Flux.Data;
using Flux.Event;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class Charge : ProxyNode
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
            Debug.Log("Starting charge for the first time");
            
            canExecute = false;
            timer = 0.0f;
        }

        protected override void OnUpdate(Packet packet)
        {
            var board = packet.Get<Blackboard>();
            float charge;
            
            if (!canExecute)
            {
                charge = board.Get<float>("charge");

                if (charge <= 0)
                {
                    canExecute = true;
                    board.Set(true, "charge.isUsed");
                }
                else return;
            }
            
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0.0f, duration);
            charge = timer / duration;

            board.Set(charge, "charge");
            Events.ZipCall(PlayerEvent.OnShake, shakeFactor * charge, maxShake);
            
            var HUD = Repository.Get<ChargeHUD>(Interface.Charge);
            HUD.Set(charge);

            IsDone = true;
        }

        protected override void OnShutdown(Packet packet)
        {
            var board = packet.Get<Blackboard>();
            board.Remove("charge");
        }
    }
}