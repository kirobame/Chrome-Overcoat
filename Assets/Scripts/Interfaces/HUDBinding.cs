using System;

namespace Chrome
{
    [Flags]
    public enum HUDBinding
    {
        None = 0,
        
        Charge = 1 << 0,
        Cooldown = 1 << 1,
        Gauge = 1 << 2,
        Ammo = 1 << 3,
        Popup = 1 << 4,
        
        Six = 1 << 5,
        Seven = 1 << 6,
        Height = 1 << 7,
        Nine = 1 << 8,
        Ten = 1 << 9
    }
}