using Flux.Data;
using Flux.Event;
using UnityEngine;

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
            handler.AddDependency(Events.Subscribe<Vector3>(GaugeEvent.OnJetpackUsed, OnJetpackUsed));
            handler.AddDependency(Events.Subscribe(GaugeEvent.OnAirControlUsed, OnAirControlUsed));
            handler.AddDependency(Events.Subscribe<byte,float>(GaugeEvent.OnDamageInflicted, OnDamageInflicted));
            handler.AddDependency(Events.Subscribe<float>(GaugeEvent.OnDamageReceived, OnDamageReceived));
            handler.AddDependency(Events.Subscribe<byte>(GaugeEvent.OnKill, OnKill));
            
            Bootup();
        }

        public void Bootup()
        {
            var module = new GaugeInRangeModule(new Vector2(0.0f, 0.01f), (value, percentage, state) =>
            {
                if (state == GaugeInRangeModule.State.EnteredRange)
                {
                    Debug.Log("Killing player due to gauge !");
                    lifetime.End();
                }
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

        void OnJetpackUsed(Vector3 displacement)
        {

        }

        void OnAirControlUsed()
        {
            
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
            }
        }
    }
}