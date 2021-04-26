using System;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using EventHandler = Flux.Event.EventHandler;

namespace Chrome
{
    public class GaugeHandler : MonoBehaviour, ILifebound
    {
        [SerializeField] private EventHandler handler;
        [SerializeField] private Lifetime lifetime;
        
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private bool heatDieOnMax = true; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private bool heatDieOnMin = false; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatConstantPassive = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatEnergyPercentPassive = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnGunFired1 = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnGunFired2 = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnJetpackUsed = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnAirControl = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnDamageInflictedConstant = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnDamageInflictedAmountMultiplier = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnDamageReceivedConstant = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnDamageReceivedAmountMultiplier = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnKill = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnJump = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnSprint = 0.0f; 
        [FoldoutGroup("Heat Gauge Settings"), SerializeField] private float heatOnMove = 0.0f; 
        
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private bool energyDieOnMax = false; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private bool energyDieOnMin = true; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyConstantPassive = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyHeatPercentPassive = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnGunFired1 = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnGunFired2 = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnJetpackUsed = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnAirControl = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnDamageInflictedConstant = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnDamageInflictedAmountMultiplier = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnDamageReceivedConstant = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnDamageReceivedAmountMultiplier = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnKill = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnJump = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnSprint = 0.0f; 
        [FoldoutGroup("Energy Gauge Settings"), SerializeField] private float energyOnMove = 0.0f; 

        private Gauge heatGauge;
        private Gauge energyGauge;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            heatGauge = Repository.Get<Gauge>(Gauges.One);
            energyGauge = Repository.Get<Gauge>(Gauges.Two);
            
            handler.AddDependency(Events.Subscribe<byte,float>(GaugeEvent.OnGunFired, OnGunFired));
            handler.AddDependency(Events.Subscribe<Vector3>(GaugeEvent.OnJetpackUsed, OnJetpackUsed));
            handler.AddDependency(Events.Subscribe<byte>(GaugeEvent.OnThrusterUsed, OnAirControlUsed));
            handler.AddDependency(Events.Subscribe<byte,float>(GaugeEvent.OnDamageInflicted, OnDamageInflicted));
            handler.AddDependency(Events.Subscribe<float>(GaugeEvent.OnDamageReceived, OnDamageReceived));
            handler.AddDependency(Events.Subscribe<byte>(GaugeEvent.OnKill, OnKill));
            handler.AddDependency(Events.Subscribe(GaugeEvent.OnJump, OnJump));
            handler.AddDependency(Events.Subscribe<byte>(GaugeEvent.OnSprint, OnSprint));
            handler.AddDependency(Events.Subscribe<byte>(GaugeEvent.OnGroundMove, OnGroundMove));
            handler.AddDependency(Events.Subscribe<byte>(GaugeEvent.OnAirMove, OnAirMove));
            
            Bootup();
        }

        public void Bootup()
        {
            if (heatDieOnMax)
            {
                // var module = new GaugeInRangeModule(new Vector2(0.99f, 1.0f), (value, percentage, state) =>
                // {
                //     if (state == GaugeInRangeModule.State.EnteredRange) lifetime.End();
                // });
                //
                // module.lifetime = new ConstantModuleLifetime();
                // heatGauge.AddModule(module);

                heatGauge.DIE(new Vector2(0.99f, 1.0f), lifetime);
            }
            
            if (energyDieOnMax) energyGauge.DIE(new Vector2(0.99f, 1.0f), lifetime);

            if (heatDieOnMin) heatGauge.DIE(new Vector2(0.0f, 0.01f), lifetime);

            if (energyDieOnMin) energyGauge.DIE(new Vector2(0.0f, 0.01f), lifetime);

            heatGauge.PASSIVE(heatConstantPassive, energyGauge, heatEnergyPercentPassive);
            energyGauge.PASSIVE(energyConstantPassive, heatGauge, energyHeatPercentPassive);
        }
        public void Shutdown() { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void OnGunFired(byte type, float force)
        {
            switch (type)
            {
                case 0: // On charged bullet fired
                    
                    heatGauge.ADD(heatOnGunFired1);
                    energyGauge.ADD(energyOnGunFired1);
                    break;
                
                case 1: // On normal bullet fired

                    heatGauge.ADD(heatOnGunFired2);
                    energyGauge.ADD(energyOnGunFired2);
                    break;
            }
        }

        void OnJetpackUsed(Vector3 displacement)
        {
            heatGauge.ADD(heatOnJetpackUsed * Time.deltaTime);
            energyGauge.ADD(energyOnJetpackUsed * Time.deltaTime);
        }

        void OnAirControlUsed(byte state)
        {
            switch (state)
            {
                case 0: // On start of use
                    
                    break;
                
                case 1: // On use
                    
                    heatGauge.ADD(heatOnAirControl * Time.deltaTime);
                    energyGauge.ADD(energyOnAirControl * Time.deltaTime);
                    break;
                
                case 2: // On end or out of time
                    
                    break;
            }
        }

        void OnDamageInflicted(byte type, float amount)
        {
            switch (type)
            {
                case 0: // On idle target hit
                    
                    break;
                
                case 1: // On enemy hit
                    
                    break;

                case 2: // On turret hit
                    
                    break;
            }
            
            heatGauge.ADD(heatOnDamageInflictedConstant + amount * heatOnDamageInflictedAmountMultiplier);
            energyGauge.ADD(energyOnDamageInflictedConstant + amount * energyOnDamageInflictedAmountMultiplier);
        }

        void OnDamageReceived(float amount)
        {
            heatGauge.ADD(heatOnDamageReceivedConstant + amount * heatOnDamageReceivedAmountMultiplier);
            energyGauge.ADD(energyOnDamageReceivedConstant + amount * energyOnDamageReceivedAmountMultiplier);
        }

        void OnKill(byte type)
        {
            switch (type)
            {
                case 0: // On idle target killed
                    
                    break;
                
                case 1: // On enemy killed
                    
                    break;
                
                case 2: // On turret killed

                    break;
            }
            
            heatGauge.ADD(heatOnKill);
            energyGauge.ADD(energyOnKill);
        }

        void OnJump()
        {
            heatGauge.ADD(heatOnJump);
            energyGauge.ADD(energyOnJump);
        }

        void OnSprint(byte state)
        {
            switch (state)
            {
                case 0: // On start of use
                    
                    break;
                
                case 1: // On use
                    
                    heatGauge.ADD(heatOnSprint * Time.deltaTime);
                    energyGauge.ADD(energyOnSprint * Time.deltaTime);
                    break;
                
                case 2: // On end or out of time
                    
                    break;
            }
        }
        
        void OnGroundMove(byte state)
        {
            switch (state)
            {
                case 0: // On start of use
                    
                    break;
                
                case 1: // On use
                    
                    heatGauge.ADD(heatOnMove * Time.deltaTime);
                    energyGauge.ADD(energyOnMove * Time.deltaTime);
                    break;
                
                case 2: // On end or out of time
                    
                    break;
            }
        }
        
        void OnAirMove(byte state)
        {
            switch (state)
            {
                case 0: // On start of use
                    
                    break;
                
                case 1: // On use
                    
                    break;
                
                case 2: // On end or out of time
                    
                    break;
            }
        }
    }
}