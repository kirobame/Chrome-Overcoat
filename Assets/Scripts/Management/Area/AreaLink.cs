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
        public Area Value { get; private set; }

        private new IValue<Collider> collider;
        private IValue<Agent> agent;
        
        private bool hasValue;

        //--------------------------------------------------------------------------------------------------------------/
        
        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup()
        {
            if (hasValue) return;

            foreach (var area in Repository.GetAll<Area>(Reference.Areas))
            {
                if (!area.Contains(collider.Value)) continue;
                
                Set(area);
                break;
            }
        }
        public void Shutdown()
        {
            Value.Unregister(agent.Value);
            
            hasValue = false;
            Value = null;
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void Set(Area area)
        {
            Value = area;
            hasValue = true;
            
            Value.Register(agent.Value);
        }

        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}