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
        [FoldoutGroup("Values"), SerializeField] private RemoteTaskTree frenzyWeapon;
        [FoldoutGroup("Values"), SerializeField] private float heatCost;
        [FoldoutGroup("Values"), SerializeField] private bool lowHeatLock;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;

        private Key key;
        private ComputeAimDirection aimCompute;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            key = Key.Default;
            
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
        
        protected override void SetupInputs() => input.Value.Bind(InputRefs.CAST, this, OnCastInput, true);
        void OnCastInput(InputAction.CallbackContext context, InputCallbackType type) => key.Update(type);

        //--------------------------------------------------------------------------------------------------------------/

        void Update()
        {
            var snapshot = packet.Save();
            if (key.State == KeyState.Down) Activate();

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
