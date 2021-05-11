using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class FrenzyAbilityControl : InputControl<FrenzyAbilityControl>, IInjectable
    {
        protected override void SetupInputs() => input.Value.BindKey(InputRefs.CAST, this, key);

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree frenzyWeapon;
        [FoldoutGroup("Values"), SerializeField] private float heatCost;
        [FoldoutGroup("Values"), SerializeField] private bool lowHeatLock;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;

        private CachedValue<Key> key;
        private ComputeAimDirection aimCompute;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            key = new CachedValue<Key>(Key.Inactive);
            
            base.Awake();
            
            identity = new AnyValue<IIdentity>();
            injections.Add(identity);

            frenzyWeapon.Bootup();
            aimCompute = ChromeExtensions.CreateComputeAimDirection();

            var board = packet.Get<IBlackboard>();
            board.Set("frenzy.heatLock", lowHeatLock);
            board.Set("frenzy.heatCost", heatCost);

            frenzyWeapon.Bootup(packet);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        void Update()
        {
            var snapshot = packet.Save();
            if (key.IsDown()) Activate();

            aimCompute.Update(packet);
            frenzyWeapon.Update(packet);

            packet.Load(snapshot);
        }

        private void Activate()
        {
            packet.Set(true);
            
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).business++;
        }
    }
}
