using System;

namespace Chrome
{
    [Serializable]
    public class HasAmmo : Condition
    {
        public HasAmmo(IValue<float> ammo) => this.ammo = ammo;

        private IValue<float> ammo;
        
        public override bool Check(Packet packet)
        {
            if (ammo.IsValid(packet)) return ammo.Value > 0.0f;
            else return false;
        }
    }
}