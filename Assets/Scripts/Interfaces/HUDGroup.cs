using System;

namespace Chrome
{
    [Flags]
    public enum HUDGroup
    {
        None = 0,

        Jetpack = 1 << 0,
        Weapon = 1 << 1,
        Timer = 1 << 2,
        Pickup = 1 << 3,
        Health = 1 << 4,
        Intensity = 1 << 5,
        
        G = 1 << 6,
        H = 1 << 7,
        I = 1 << 8,
        J = 1 << 9
    }
}