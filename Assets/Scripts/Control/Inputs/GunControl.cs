using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private const string MELEE = "Melee";

        //--------------------------------------------------------------------------------------------------------------/
        
        protected override void PrepareInjection()
        {
            hasBeenBootedUp = false;
            
            aimCompute = ChromeExtensions.CreateComputeAimDirection();
            runtimeDefaultWeapon = Instantiate(defaultWeapon);
            runtimeDefaultWeapon.Build();

            pressState = PressState.Released;
            switchState = SwitchState.None;
            holsterTimer = 0.0f;
            takeOutTimer = 0.0f;
            
            HasWeapon = false;
            Current = null;
            
            identity = injections.Register(new AnyValue<IIdentity>());
            animator = injections.Register(new AnyValue<Animator>());
            visual = injections.Register(new AnyValue<WeaponVisual>());
        }
        protected override void OnInjectionDone(IRoot source)
        {
            packet.Set(false);

            hasBeenBootedUp = true;
            SwitchTo(runtimeDefaultWeapon);
        }
        
        protected override void SetupInputs()
        {
            shootKey = new CachedValue<Key>(Key.Inactive);
            input.Value.BindKey(InputRefs.SHOOT, this, shootKey);
        }

        //--------------------------------------------------------------------------------------------------------------/

        public bool HasDefaultWeapon => Current == runtimeDefaultWeapon;
        
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
        
        private bool weaponHasAmmo;
        private Bindable<float> ammoBinding;
        
        private bool hasBeenBootedUp;

        //--------------------------------------------------------------------------------------------------------------/
        
        public override void Bootup(byte code)
        {
            if (Current.IsMelee) animator.Value.SetTrigger(MELEE);
            packet.Set(false);
            
            base.Bootup(code);
        }
        public override void Shutdown(byte code)
        {
            base.Shutdown(code);
            
            if (pressState == PressState.Pressed) OnMouseUp();
            if (Current != runtimeDefaultWeapon)
            {
                targetWeapon = runtimeDefaultWeapon;
                Refresh();
            }
        }

        //--------------------------------------------------------------------------------------------------------------/

        public void DropCurrent() => SwitchTo(runtimeDefaultWeapon);
        public void SwitchTo(Weapon weapon)
        {
            if (switchState == SwitchState.None) // Launch normally
            {
                if (Current == weapon) return;
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
            
            if (Current.IsMelee)
            {
                takeOutTimer = 0.0f;
                
                Refresh();
                switchRoutine = StartCoroutine(TakeOutRoutine());
                
                yield break;
            }
            
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

            if (Current.IsMelee)
            {
                animator.Value.SetTrigger(MELEE);
                yield return new WaitForEndOfFrame();
                
                switchState = SwitchState.None;
                switchRoutine = null;

                yield break;
            }
            
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
            if (targetWeapon == null) throw new InvalidOperationException($"[{this}] Cannot refresh to a new weapon if there is no target weapon assigned!");

            if (HasWeapon)
            {
                HUDBinder.Clear(HUDGroup.Weapon);
                Current.Shutdown(packet);

                if (weaponHasAmmo) ammoBinding.onChange -= OnAmmoChange;
            }
            HasWeapon = true;
            
            Current = targetWeapon;
            targetWeapon = null;

            packet.Set(false);
            
            var board = packet.Get<IBlackboard>();
            board.Set(WeaponRefs.BOARD, Current.Board);
            
            Current.Bootup(packet);
            if (!Current.IsMelee) Current.AssignVisualsTo(visual.Value);

            var bindables = Current.GetBindables();
            if (!bindables.Any())
            {
                weaponHasAmmo = false;
                return;
            }
            
            HUDBinder.Declare(HUDGroup.Weapon, bindables);
            if (bindables.TryGet<Bindable<float>>(HUDBinding.Ammo, out ammoBinding))
            {
                weaponHasAmmo = true;
                ammoBinding.onChange += OnAmmoChange;
            }
            else weaponHasAmmo = false;
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

        //--------------------------------------------------------------------------------------------------------------/

        void OnAmmoChange(float value)
        {
            if (Current == runtimeDefaultWeapon || value > 0) return;
            DropCurrent();
        }
    }
}