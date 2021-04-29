using System;
using System.Collections.Generic;
using Flux.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using EventHandler = Flux.Event.EventHandler;

namespace Chrome.Retro
{
    public class RetGaugeSignalHandler : MonoBehaviour
    {
        #region Nested Types

        private enum Signal
        {
            Inflict,
            Received,
            Kill
        }
        
        [Serializable]
        private struct Modification
        {
            public Signal signal;
            
            [FoldoutGroup("Core"), HideIf("signal", Signal.Received)] public byte wantedTarget;
            [FoldoutGroup("Core"), HideIf("signal", Signal.Received), HideIf("signal", Signal.Kill)] public float targetMultiplier;
            [FoldoutGroup("Core"), HideIf("signal", Signal.Received)] public float targetAddition;
            
            [FoldoutGroup("Core")] public byte wantedSource;
            [FoldoutGroup("Core"), HideIf("signal", Signal.Kill)] public float sourceMultiplier;
            [FoldoutGroup("Core")] public float sourceAddition;
        }

        #endregion

        private List<Modification> inflictModifications = new List<Modification>();
        private List<Modification> receiveModifications = new List<Modification>();
        private List<Modification> killModifications = new List<Modification>();

        [SerializeField] public bool isActive = true;
        
        [FoldoutGroup("Dependencies"), SerializeField] private EventHandler handler;
        [FoldoutGroup("Dependencies"), SerializeField] private RetGauge gauge;

        [SerializeField] private Modification[] modifications;
        
        void Awake()
        {
            if (!isActive) return;
            
            foreach (var modification in modifications)
            {
                switch (modification.signal)
                {
                    case Signal.Inflict:

                        inflictModifications.Add(modification);
                        break;

                    case Signal.Received:

                        receiveModifications.Add(modification);
                        break;

                    case Signal.Kill:

                        killModifications.Add(modification);
                        break;
                }
            }
            modifications = Array.Empty<Modification>();
            
            handler.AddDependency(Events.Subscribe<byte,float,byte>(GaugeEvent.OnDamageInflicted, OnDamageInflicted));
            handler.AddDependency(Events.Subscribe<float,byte>(GaugeEvent.OnDamageReceived, OnDamageReceived));
            handler.AddDependency(Events.Subscribe<byte,byte>(GaugeEvent.OnKill, OnKill));
        }
        
        void OnDamageInflicted(byte target, float amount, byte source)
        {
            foreach (var inflModif in inflictModifications)
            {
                if (inflModif.wantedSource == 0 && inflModif.wantedTarget == 0) gauge.Modify(inflModif.targetAddition + inflModif.sourceAddition);
                else if (inflModif.wantedTarget == target)
                {
                    if (inflModif.wantedSource == 0) gauge.Modify(inflModif.targetMultiplier * amount + inflModif.targetAddition);
                    else if (inflModif.wantedSource == source)
                    {
                        var input = inflModif.targetMultiplier * inflModif.sourceMultiplier * amount;
                        input += inflModif.targetAddition + inflModif.sourceAddition;
                        
                        gauge.Modify(input);
                    }
                }
                else if (inflModif.wantedSource == source)
                {
                    if (inflModif.wantedTarget == 0) gauge.Modify(inflModif.sourceMultiplier * amount + inflModif.sourceAddition);
                    else if (inflModif.wantedTarget == target)
                    {
                        var input = inflModif.targetMultiplier * inflModif.sourceMultiplier * amount;
                        input += inflModif.targetAddition + inflModif.sourceAddition;
                        
                        gauge.Modify(input);
                    }
                }
            }
        }
        
        void OnDamageReceived(float amount, byte source)
        {
            foreach (var recModif in receiveModifications)
            {
                if (recModif.wantedSource == 0 || recModif.wantedSource == source) gauge.Modify(recModif.sourceMultiplier * amount + recModif.sourceAddition);
            }
        }
        
        void OnKill(byte target, byte source)
        {
            foreach (var killModif in killModifications)
            {
                if (killModif.wantedTarget == target)
                {
                    if (killModif.wantedSource == 0) gauge.Modify(killModif.targetAddition);
                    else if (killModif.wantedSource == source) gauge.Modify(killModif.targetAddition + killModif.sourceAddition);
                }
                else if (killModif.wantedSource == source)
                {
                    if (killModif.wantedTarget == 0) gauge.Modify(killModif.sourceAddition);
                    else if (killModif.wantedTarget == target) gauge.Modify(killModif.targetAddition + killModif.sourceAddition);
                }
            }
        }
    }
}