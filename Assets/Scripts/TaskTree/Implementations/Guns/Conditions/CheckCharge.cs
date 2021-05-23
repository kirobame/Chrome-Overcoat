using System;
using System.Collections;
using Flux;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class CheckCharge : Condition
    {
        public CheckCharge(BindableCappedGauge gaugeBinding, float decrease)
        {
            this.gaugeBinding = gaugeBinding;
            this.decrease = decrease;
        }

        [SerializeField] private float decrease;

        private BindableCappedGauge gaugeBinding;
        private Coroutine cooldownRoutine;

        public override void Bootup(Packet packet) => gaugeBinding.Value = 0.0f;

        public override bool Check(Packet packet)
        {
            if (cooldownRoutine != null) return false;
            
            if (gaugeBinding.IsUsable)
            {
                cooldownRoutine = Routines.Start(CooldownRoutine(packet));
                return true;
            }
            else
            {
                gaugeBinding.Value = 0.0f;
                return false;
            }
        }

        public override void Shutdown(Packet packet)
        {
            if (cooldownRoutine != null)
            {
                Routines.Stop(cooldownRoutine);
                cooldownRoutine = null;
            }

            gaugeBinding.Value = 0.0f;
            gaugeBinding.IsLocked = false;
        }

        private IEnumerator CooldownRoutine(Packet packet)
        {
            gaugeBinding.IsLocked = true;
            
            while (gaugeBinding.Value > 0.0f)
            {
                gaugeBinding.Value -= Time.deltaTime * decrease;
                yield return new WaitForEndOfFrame();
            }

            gaugeBinding.IsLocked = false;
            cooldownRoutine = null;
        }
    }
}