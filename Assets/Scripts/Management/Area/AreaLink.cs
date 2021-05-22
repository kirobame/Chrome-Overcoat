using System;
using System.Collections.Generic;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class AreaLink : MonoBehaviour, ILifebound, IInstaller, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            collider = new AnyValue<Collider>();
            agent = new AnyValue<Agent>();
            
            injections = new IValue[]
            {
                collider,
                agent
            };
        }

        //--------------------------------------------------------------------------------------------------------------/
        
        public event Action<ILifebound> onDestruction;

        public bool IsActive => enabled;
        public Area Area { get; private set; }

        private new IValue<Collider> collider;
        private IValue<Agent> agent;
        
        private bool isInArea;

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup()
        {
            if (isInArea) return;

            foreach (var area in Repository.GetAll<Area>(Reference.Areas))
            {
                if (!area.Contains(collider.Value)) continue;
                
                Set(area);
                break;
            }
        }
        public void Shutdown()
        {
            Area.Unregister(agent.Value);
            
            isInArea = false;
            Area = null;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void Set(Area area)
        {
            Area = area;
            isInArea = true;
            
            Area.Register(agent.Value);
        }

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}