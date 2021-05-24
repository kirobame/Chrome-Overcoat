using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chrome
{
    public class IntensityControl : MonoBehaviour, ILifebound, IInjectable, IInstaller
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };
        }

        //--------------------------------------------------------------------------------------------------------------/

        public event Action<ILifebound> onDestruction;
        
        public bool IsActive => true;
        public float Value
        {
            get => gauge.Value;
            set => gauge.Value = value;
        }
        
        [SerializeReference] private IIntensityPassive[] passives = new IIntensityPassive[0];

        [HideInInspector] public float affect;

        private IValue<IIdentity> identity;
        
        private BindableGauge gauge;

        //--------------------------------------------------------------------------------------------------------------/
        
        void Start()
        {
            gauge = new BindableGauge(HUDBinding.Gauge, 0.0f, new Vector2(0.0f, 100.0f));
            HUDBinder.Declare(HUDGroup.Intensity, gauge);
        }
        void OnDestroy() => onDestruction?.Invoke(this);

        public void Bootup(byte code)
        {
            gauge.Value = 0.0f;
            foreach (var passive in passives) passive.Bootup(identity.Value);
        }
        public void Shutdown(byte code) { foreach (var passive in passives) passive.Shutdown(identity.Value); }

        void Update()
        {
            var previous = Value;
            Value += affect;
            
            foreach (var passive in passives)
            {
                if (passive.Range.Contains(Value))
                {
                    var ratio = ComputeRatio(passive);
                    
                    if (!passive.Range.Contains(previous)) passive.Start(Value, ratio, identity.Value);
                    passive.Update(Value, ratio, identity.Value);
                }
                else if (passive.Range.Contains(previous)) passive.End(Value, ComputeRatio(passive), identity.Value);
            }

            affect = 0.0f;
        }

        private float ComputeRatio(IIntensityPassive passive) => Mathf.InverseLerp(passive.Range.x, passive.Range.y, Value);
        
        //--------------------------------------------------------------------------------------------------------------/

        int IInstaller.Priority => 1;

        void IInstaller.InstallDependenciesOn(Packet packet) => packet.Set(this);
    }
}