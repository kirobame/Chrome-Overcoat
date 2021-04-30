using System;
using Flux.Data;
using Flux.Event;
using UnityEngine;
using EventHandler = Flux.Event.EventHandler;

namespace Chrome
{
    public class GaugeHandler : MonoBehaviour, ILifebound
    {
        [SerializeField] private EventHandler handler;
        [SerializeField] private Lifetime lifetime;

        private Gauge firstGauge;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            firstGauge = Repository.Get<Gauge>(Gauges.One);
            
            handler.AddDependency(Events.Subscribe<byte,float>(GaugeEvent.OnGunFired, OnGunFired));
            handler.AddDependency(Events.Subscribe<byte,float>(GaugeEvent.OnFrenzyAbilityUsed, OnFrenzyAbilityUsed));
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
            var module = new GaugeInRangeModule(new Vector2(0.0f, 0.01f), (value, percentage, state) =>
            {
                if (state == GaugeInRangeModule.State.EnteredRange) lifetime.End();
            });
            
            module.lifetime = new ConstantModuleLifetime();
            firstGauge.AddModule(module);
        }
        public void Shutdown() { }
        
        //--------------------------------------------------------------------------------------------------------------/
        
        void OnGunFired(byte type, float force)
        {
            switch (type)
            {
                case 0: // On charged bullet fired
                    
                    break;
                
                case 1: // On normal bullet fired

                    firstGauge.ADD(-2.5f);
                    break;
            }
        }
        void OnFrenzyAbilityUsed(byte type, float cost)
        {
            firstGauge.ADD(cost);
            switch (type)
            {
                case 0:
                    break;
            }
        }

        void OnJetpackUsed(Vector3 displacement)
        {

        }

        void OnAirControlUsed(byte state)
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

        void OnDamageInflicted(byte type, float amount)
        {
            switch (type)
            {
                case 0: // On idle target hit
                    
                    firstGauge.ADD(5.0f * amount);
                    break;
                
                case 1: // On enemy hit
                    
                    firstGauge.ADD(7.5f * amount);
                    break;

                case 2: // On turret hit

                    firstGauge.ADD(7.5f * amount);
                    break;

                case 3: // On shield hit

                    firstGauge.ADD(-5f * amount);
                    break;
            }
        }

        void OnDamageReceived(float amount) => firstGauge.ADD(-amount);

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
        }

        void OnJump()
        {
            
        }

        void OnSprint(byte state)
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
        
        void OnGroundMove(byte state)
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