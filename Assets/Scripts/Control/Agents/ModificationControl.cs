using System.Collections.Generic;
using Flux;
using UnityEngine;

namespace Chrome
{
    public class ModificationControl : MonoBehaviour, IInjectable, IInjectionCallbackListener
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;

        void IInjectable.PrepareInjection()
        {
            agent = new AnyValue<Agent>();
            injections = new IValue[] { agent};
        }

        void IInjectionCallbackListener.OnInjectionDone(IRoot source)
        {
            Routines.Start(Routines.DoAfter(() =>
            {
                foreach (var modification in modifications) modification.Modify(agent.Value);
                
            }, new YieldFrame())); }

        //--------------------------------------------------------------------------------------------------------------/

        [SerializeReference] private IAgentModification[] modifications = new IAgentModification[0];

        private IValue<Agent> agent;
    }
}