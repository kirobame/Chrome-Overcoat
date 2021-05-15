using System;
using System.Collections;
using System.Collections.Generic;
using Flux.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Chrome
{
    public class GunControl : InputControl<GunControl>
    {
        #region Nested Types

        private enum PressState : byte
        {
            Pressed,
            Released,
        }

        private enum SwitchState : byte
        {
            None,
            Holstering,
            TakingOut
        }
        #endregion

        private const string HOLSTER = "Holster";
        private const string HOLSTER_RATIO = "HolsterRatio";
        private const string TAKE_OUT = "TakeOut";
        private const string TAKE_OUT_RATIO = "TakeOutRatio";

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void SetupInputs() => input.Value.BindKey(InputRefs.SHOOT, this, shootKey);

        protected override void OnInjectionDone(IRoot source)
        {
            hasBeenBootedUp = true;
            
            packet.Set(false);
            SwitchTo(runtimeDefaultWeapon);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public bool HasWeapon { get; private set; }
        public Weapon Current { get; private set; }
        
        [FoldoutGroup("Values"), SerializeField] private Weapon defaultWeapon;
        [FoldoutGroup("Values"), SerializeField] private Vector2 switchTiming;

        private Packet packet => identity.Value.Packet;
        
        private IValue<IIdentity> identity;
        private IValue<Animator> animator;
        private IValue<WeaponVisual> visual;
        
        private CachedValue<Key> shootKey;
        private ComputeAimDirection aimCompute;

        private PressState pressState;
        private SwitchState switchState;
        private Coroutine switchRoutine;
        private float holsterTimer;
        private float takeOutTimer;

        private Weapon targetWeapon;
        private Weapon runtimeDefaultWeapon;
        
        private bool hasBeenBootedUp;

        //--------------------------------------------------------------------------------------------------------------/

        protected override void Awake()
        {
            hasBeenBootedUp = false;
            shootKey = new CachedValue<Key>(Key.Inactive);
            
            base.Awake();
            
            identity = injections.Register(new AnyValue<IIdentity>());
            animator = injections.Register(new AnyValue<Animator>());
            visual = injections.Register(new AnyValue<WeaponVisual>());

            runtimeDefaultWeapon = Instantiate(defaultWeapon);
            runtimeDefaultWeapon.Build();
            aimCompute = ChromeExtensions.CreateComputeAimDirection();

            pressState = PressState.Released;
            switchState = SwitchState.None;
            holsterTimer = 0.0f;
            takeOutTimer = 0.0f;
            HasWeapon = false;
        }

        public override void Bootup()
        {
            packet.Set(false);
            base.Bootup();
        }
        public override void Shutdown()
        {
            base.Shutdown();
            if (pressState == PressState.Pressed) OnMouseUp();
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void DropCurrent() => SwitchTo(runtimeDefaultWeapon);
        public void SwitchTo(Weapon weapon)
        {
            if (switchState == SwitchState.None) // Launch normally
            {
                targetWeapon = weapon;

                if (HasWeapon) switchRoutine = StartCoroutine(HolsterRoutine());
                else
                {
                    Refresh();
                    switchRoutine = StartCoroutine(TakeOutRoutine());
                }
            }
            else if (switchState == SwitchState.Holstering)
            {
                if (Current == weapon) // Cancel and go back to current
                {
                    StopCoroutine(switchRoutine);
                    switchRoutine = StartCoroutine(TakeOutRoutine());
                }
                else targetWeapon = weapon; // Simply change target weapon
            }
            else if (switchState == SwitchState.TakingOut) // Cancel and restart with new weapon
            {
                targetWeapon = weapon;
                
                StopCoroutine(switchRoutine);
                switchRoutine = StartCoroutine(HolsterRoutine());
            }
        }

        private IEnumerator HolsterRoutine()
        {
            switchState = SwitchState.Holstering;
            
            animator.Value.SetBool(TAKE_OUT, false);
            animator.Value.SetBool(HOLSTER, true);
            yield return new WaitForEndOfFrame();
            
            while (true)
            {
                holsterTimer += Time.deltaTime;
                var ratio = Mathf.Clamp01(holsterTimer / switchTiming.x);
                
                animator.Value.SetFloat(HOLSTER_RATIO, ratio);
                takeOutTimer = switchTiming.y * (1.0f - ratio);

                if (holsterTimer >= switchTiming.x) break;
                yield return new WaitForEndOfFrame();
            }

            Refresh();
            animator.Value.SetBool(HOLSTER, false);
            switchRoutine = StartCoroutine(TakeOutRoutine());
        }
        private IEnumerator TakeOutRoutine()
        {
            switchState = SwitchState.TakingOut;

            animator.Value.SetBool(HOLSTER, false);
            animator.Value.SetBool(TAKE_OUT, true);
            yield return new WaitForEndOfFrame();
            
            while (true)
            {
                takeOutTimer += Time.deltaTime;
                var ratio = Mathf.Clamp01(takeOutTimer / switchTiming.y);
                
                animator.Value.SetFloat(TAKE_OUT_RATIO, ratio);
                holsterTimer = switchTiming.x * (1.0f - ratio);

                if (takeOutTimer >= switchTiming.y) break;
                yield return new WaitForEndOfFrame();
            }

            animator.Value.SetBool(TAKE_OUT, false);
            switchState = SwitchState.None;
            switchRoutine = null;
        }

        private void Refresh()
        {
            if (targetWeapon == null) throw new InvalidOperationException($"[{this}] Cannot refresh to a new weapon if there is no target weapon assigned !");
            
            if (HasWeapon) Current.Shutdown(packet);
            HasWeapon = true;
            
            Current = targetWeapon;
            targetWeapon = null;
            
            Current.Bootup(packet);
            Current.AssignVisualsTo(visual.Value);
        }
        
        //--------------------------------------------------------------------------------------------------------------/

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus || !hasBeenBootedUp) return;
            OnMouseDown();
        }
        
        void Update()
        {
            if (!HasWeapon || switchState != SwitchState.None) return;
            
            var snapshot = packet.Save();

            if (pressState == PressState.Released)
            {
                if (shootKey.IsDown())
                {
                    packet.Set(true);
                    OnMouseDown();
                }
            }
            else if (pressState == PressState.Pressed)
            {
                if (shootKey.IsActive()) packet.Set(true);

                if (shootKey.IsUp())
                {
                    packet.Set(false);
                    OnMouseUp();
                }
            }

            aimCompute.Use(packet);
            Current.Actualize(packet);

            packet.Load(snapshot);
        }

        private void OnMouseDown()
        {
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).business++;
            
            pressState = PressState.Pressed;
        }
        private void OnMouseUp()
        {
            var board = packet.Get<IBlackboard>();
            board.Get<BusyBool>(PlayerRefs.CAN_SPRINT).business--;
            
            pressState = PressState.Released;
        }
    }
}