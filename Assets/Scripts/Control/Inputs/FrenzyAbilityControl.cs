using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Chrome
{
    public class FrenzyAbilityControl : InputControl<FrenzyAbilityControl>, IInjectable
    {
        IReadOnlyList<IValue> IInjectable.Injections => injections;
        private IValue[] injections;
        
        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree frenzyWeapon;
        [FoldoutGroup("Values"), SerializeField] private float heatCost;
        [FoldoutGroup("Values"), SerializeField] private bool lowHeatLock;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;
        
        private ComputeAimDirection aimCompute;

        void Awake()
        {
            identity = new AnyValue<IIdentity>();
            injections = new IValue[] { identity };

            frenzyWeapon.Bootup();
            aimCompute = ChromeExtensions.CreateComputeAimDirection();

            var board = packet.Get<IBlackboard>();
            board.Set("frenzy.heatLock", lowHeatLock);
            board.Set("frenzy.heatCost", heatCost);

            frenzyWeapon.Bootup(packet);
        }

        void Update()
        {
            var snapshot = packet.Save();

            if (Input.GetKeyDown(KeyCode.A)) Activate();

            aimCompute.Update(packet);
            frenzyWeapon.Update(packet);

            packet.Load(snapshot);
        }

        private void Activate()
        {
            packet.Set(true);
            
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>("canSprint").business++;
        }
    }
}
