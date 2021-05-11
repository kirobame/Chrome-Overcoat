using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class FrenzyAbilityControl : InputControl<FrenzyAbilityControl>
    {
        protected override void OnInjectionDone(IRoot source)
        {
            var board = packet.Get<IBlackboard>();
            board.Set("frenzy.heatLock", lowHeatLock);
            board.Set("frenzy.heatCost", heatCost);
            
            runtimeWeapon.Bootup(packet);
        }

        protected override void SetupInputs() => input.Value.BindKey(InputRefs.CAST, this, key);

        //--------------------------------------------------------------------------------------------------------------/
        
        [FoldoutGroup("Values"), SerializeField] private Weapon frenzyWeapon;
        [FoldoutGroup("Values"), SerializeField] private float heatCost;
        [FoldoutGroup("Values"), SerializeField] private bool lowHeatLock;

        private Packet packet => identity.Value.Packet;
        private IValue<IIdentity> identity;

        private CachedValue<Key> key;

        private Weapon runtimeWeapon;
        private ComputeAimDirection aimCompute;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            key = new CachedValue<Key>(Key.Inactive);
            
            base.Awake();
            
            identity = new AnyValue<IIdentity>();
            injections.Add(identity);
            
            aimCompute = ChromeExtensions.CreateComputeAimDirection();
            runtimeWeapon = Instantiate(frenzyWeapon);
        }
        void Start() => runtimeWeapon.Build();
        
        //--------------------------------------------------------------------------------------------------------------/

        void Update()
        {
            var snapshot = packet.Save();
            if (key.IsDown()) Activate();

            aimCompute.Use(packet);
            runtimeWeapon.Actualize(packet);

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