using System;
using UnityEngine;

namespace Chrome
{
    [Serializable]
    public class HasAmmo : Condition
    {
        public HasAmmo(IValue<float> ammo) => this.ammo = ammo;

        private IValue<float> ammo;
        
        public override bool Check(Packet packet)
        {
            var bb = packet.Get<IBlackboard>();
            //Debug.Log("HasAmmo");
            if (ammo.IsValid(packet)) return ammo.Value > 0.0f;
            else
            {
                Debug.Log($"not valid {bb.Get<IBlackboard>(WeaponRefs.BOARD)}" );
                return false;
            }
        }
    }
}